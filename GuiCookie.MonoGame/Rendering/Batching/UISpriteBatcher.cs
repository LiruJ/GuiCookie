using Microsoft.Xna.Framework.Graphics;
using System;

namespace GuiCookie.MonoGame.Rendering.Batching
{
    internal class UISpriteBatcher
    {
        #region Constants
        private const int defaultBatchSize = 256;

        private const int maxBatchSize = short.MaxValue / 6;
        #endregion

        #region Dependencies
        private readonly GraphicsDevice graphicsDevice;
        #endregion

        #region Fields
        private VertexPositionColorTextureUI[] vertices;

        private short[] indices;

        private UIBatchItem[] batchItems;

        private int nextBatchItemIndex = 0;
        #endregion

        #region Constructors
        public UISpriteBatcher(GraphicsDevice graphicsDevice, int capacity = 0)
        {
            this.graphicsDevice = graphicsDevice;

            capacity = capacity <= 0 ? defaultBatchSize : (capacity + 63) & (~63);

            batchItems = new UIBatchItem[capacity];
            for (int i = 0; i < capacity; i++)
                batchItems[i] = new UIBatchItem();

            vertices = new VertexPositionColorTextureUI[4 * capacity];
            indices = Array.Empty<short>();

            ensureBatchItemsCapacity(capacity);
        }
        #endregion

        #region Batch Functions
        public UIBatchItem GetBatchItem()
        {
            if (nextBatchItemIndex >= batchItems.Length)
            {
                int oldSize = batchItems.Length;
                int newSize = oldSize + (oldSize / 2); // grow by x1.5
                newSize = (newSize + 63) & (~63); // grow in chunks of 64.
                Array.Resize(ref batchItems, newSize);
                for (int i = oldSize; i < newSize; i++)
                    batchItems[i] = new UIBatchItem();

                ensureBatchItemsCapacity(Math.Min(newSize, maxBatchSize));
            }

            return batchItems[nextBatchItemIndex++];
        }

        private unsafe void ensureBatchItemsCapacity(int numBatchItems)
        {
            int neededCapacity = 6 * numBatchItems;
            if (indices != null && neededCapacity <= indices.Length)
                return;

            int startIndex = 0;
            if (indices != null)
            {
                startIndex = indices.Length / 6;
                Array.Resize(ref indices, 6 * numBatchItems);
            }

            fixed (short* indexFixedPtr = indices)
            {
                short* indexPtr = indexFixedPtr + (startIndex * 6);
                for (int i = startIndex; i < numBatchItems; i++, indexPtr += 6)
                {
                    /*
                     *  TL    TR
                     *   0----1 0,1,2,3 = index offsets for vertex indices
                     *   |   /| TL,TR,BL,BR are vertex references in SpriteBatchItem.
                     *   |  / |
                     *   | /  |
                     *   |/   |
                     *   2----3
                     *  BL    BR
                     */
                    // Triangle 1
                    *(indexPtr + 0) = (short)(i * 4);
                    *(indexPtr + 1) = (short)(i * 4 + 1);
                    *(indexPtr + 2) = (short)(i * 4 + 2);
                    // Triangle 2
                    *(indexPtr + 3) = (short)(i * 4 + 1);
                    *(indexPtr + 4) = (short)(i * 4 + 3);
                    *(indexPtr + 5) = (short)(i * 4 + 2);
                }
            }

            Array.Resize(ref vertices, 4 * numBatchItems);
        }
        #endregion

        #region Draw Functions
        public unsafe void DrawBatch(SpriteSortMode spriteSortMode, GuiCookieEffect? effect)
        {
            ObjectDisposedException.ThrowIf(effect == null || effect.IsDisposed, typeof(GuiCookieEffect));

            if (nextBatchItemIndex == 0)
                return;

            switch (spriteSortMode)
            {
                case SpriteSortMode.Texture:
                case SpriteSortMode.FrontToBack:
                case SpriteSortMode.BackToFront:
                    Array.Sort(batchItems, 0, nextBatchItemIndex);
                    break;
            }

            // Determine how many iterations through the drawing code we need to make
            int batchIndex = 0;
            int batchCount = nextBatchItemIndex;

            // Iterate through the batches, doing short.MaxValue sets of vertices only.
            while (batchCount > 0)
            {
                // setup the vertexArray array
                int startIndex = 0;
                int index = 0;
                Texture2D? texture = null;

                int numBatchesToProcess = Math.Min(batchCount, maxBatchSize);

                // Avoid the array checking overhead by using pointer indexing!
                fixed (VertexPositionColorTextureUI* vertexArrayFixedPtr = vertices)
                {
                    VertexPositionColorTextureUI* vertexArrayPtr = vertexArrayFixedPtr;

                    // Draw the batches
                    for (int i = 0; i < numBatchesToProcess; i++, batchIndex++, index += 4, vertexArrayPtr += 4)
                    {
                        UIBatchItem item = batchItems[batchIndex];

                        // if the texture changed, we need to flush and bind the new texture
                        bool shouldFlush = !ReferenceEquals(item.Texture, texture);

                        if (shouldFlush)
                        {
                            flushVertexArray(startIndex, index, effect, texture!);

                            texture = item.Texture;
                            startIndex = index = 0;
                            vertexArrayPtr = vertexArrayFixedPtr;
                            graphicsDevice.Textures[0] = texture;
                        }

                        // store the SpriteBatchItem data in our vertexArray
                        *(vertexArrayPtr + 0) = item.vertexTL;
                        *(vertexArrayPtr + 1) = item.vertexTR;
                        *(vertexArrayPtr + 2) = item.vertexBL;
                        *(vertexArrayPtr + 3) = item.vertexBR;

                        // Release the texture.
                        item.Texture = null;
                    }
                }
                // flush the remaining vertexArray data
                flushVertexArray(startIndex, index, effect, texture!);
                // Update our batch count to continue the process of culling down
                // large batches
                batchCount -= numBatchesToProcess;
            }
            // return items to the pool.  
            nextBatchItemIndex = 0;
        }

        private void flushVertexArray(int start, int end, GuiCookieEffect? effect, Texture texture)
        {
            if (start == end)
                return;

            int vertexCount = end - start;

            if (effect == null)
            {
                drawPrimitives(vertexCount);
                return;
            }

            // If the effect is not null, then apply each pass and render the geometry
            EffectPassCollection passes = effect.CurrentTechnique.Passes;
            foreach (EffectPass? pass in passes)
            {
                pass.Apply();

                // Whatever happens in pass.Apply, make sure the texture being drawn
                // ends up in Textures[0].
                graphicsDevice.Textures[0] = texture;

                drawPrimitives(vertexCount);
            }
        }

        private void drawPrimitives(int vertexCount)
        {
            graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                vertices,
                0,
                vertexCount,
                indices,
                0,
                (vertexCount / 4) * 2,
                VertexPositionColorTextureUI.VertexDeclaration);
        }
        #endregion
    }
}
