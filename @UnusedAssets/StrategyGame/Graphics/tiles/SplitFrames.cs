            //SpitFrames("grassgrid.png", "Grass");
            //SpitFrames("dirtgrid.png", "Dirt");
            //SpitFrames("rocksgrid.png", "Rock");
            //SpitFrames("shoregrid.png", "Shore");
            SpitFrames("watergrid.png", "Water");

  void SpitFrames(string source, string name)
        {
            source = @"C:\NTDLS\RougueQuest\Assets\Terrain\@Unused\StrategyGame\Graphics\tiles\" + source;
            string target = @"C:\NTDLS\RougueQuest\Assets\Terrain\@Unused\StrategyGame\Graphics\tiles\" + name + "\\";


            var sheet = Bitmap.FromFile(source) as Bitmap;

            Rectangle cropRect = new Rectangle(1, 1, sheet.Width - 1, sheet.Height - 1);

            sheet = sheet.Clone(cropRect, sheet.PixelFormat);

            int columnPadding = 0;
            int rowPadding = 0;

            int height = 32;
            int width = 32;

            int columns = 13;
            int rows = 5;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    var startX = (column * (width + columnPadding));
                    var startY = (row * (height + rowPadding));

                    Rectangle cloneRect = new Rectangle(startX, startY, width, height);
                    var frame = sheet.Clone(cloneRect, sheet.PixelFormat);
                    frame.Save(target + "\\" + $"{row}-{column}.png", ImageFormat.Png);

                    columnPadding = 1;

                }
                rowPadding = 1;
            }
        }