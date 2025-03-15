using SkiaSharp;
using System.Globalization;

namespace PorcupineBot.Services 
{
    public static class ImageGenerator
    {
        private static readonly string POPPINS_REGULAR = "Fonts/Poppins-Regular.ttf";
        private static readonly string POPPINS_BOLD = "Fonts/Poppins-Bold.ttf";
        private static readonly string POPPINS_MEDIUM = "Fonts/Poppins-Medium.ttf";

        private static int Width = 1200; 
        private static int Height = 900; 

        public static string FilePath
        {
            get
            {
                string projectRootPath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(projectRootPath, "discord_ranking.png"); 
                return filePath;
            }
        }

        public static Stream GetImageStream()
        {   
            if (File.Exists(FilePath))
            { 
                byte[] imageBytes = File.ReadAllBytes(FilePath);
                return new MemoryStream(imageBytes); 
            }
            else
            {
                throw new FileNotFoundException("The image was not found in the directory");
            }
        }
         
        public static void GenerateRankingImage(string serverName, string rakingName, string[,] rankinInfor)
        {
            using (SKSurface surface = SKSurface.Create(new SKImageInfo(Width, Height)))
            {
                SKCanvas canvas = surface.Canvas;
                canvas.Clear(new SKColor(32, 34, 37));

                DrawHeader(canvas, serverName, rakingName);
                DrawSection(canvas, rankinInfor);

                DrawFooter(canvas);

                using (SKImage image = surface.Snapshot())
                {
                    using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {  
                        using (FileStream stream = File.OpenWrite(FilePath)) 
                        {
                            data.SaveTo(stream);
                        } 
                    } 
                }
            }
        }
 
        public static async Task DownloadIcon(string imageUrl, string serverName)
        {
            string projectRootPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(projectRootPath, $"{serverName.ToLower()}.png");

            if (!File.Exists(filePath))
            {
                using (var client = new HttpClient())
                {
                    var imageBytes = await client.GetByteArrayAsync(imageUrl);
                    await File.WriteAllBytesAsync(filePath, imageBytes);
                }
            }
        } 

        private static void DrawHeader(SKCanvas canvas, string serverName, string rankingName)
        {
            using (SKSurface iconSurface = SKSurface.Create(new SKImageInfo(80, 80)))
            { 
                string projectRootPath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(projectRootPath, $"{serverName.ToLower()}.png");
                 
                byte[] imageBytes = File.ReadAllBytes(filePath);

                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    using (SKBitmap bitmap = SKBitmap.Decode(filePath))
                    {
                        SKCanvas iconCanvas = iconSurface.Canvas;

                        using (SKPaint paint = new SKPaint { Color = new SKColor(0, 102, 0) })
                        {
                            iconCanvas.DrawRect(0, 0, 80, 80, paint);
                        }

                        iconCanvas.DrawBitmap(bitmap, new SKRect(0, 0, 80, 80));

                        using (SKImage iconImage = iconSurface.Snapshot())
                        {
                            canvas.DrawImage(iconImage, 20, 30);
                        }
                    }
                } 
            }

            using (SKPaint titlePaint = new SKPaint
            {
                Color = SKColors.White, 
                IsAntialias = true,
            })
            {
                canvas.DrawText(CreateText(serverName, 37, true), 120, 65, titlePaint);
            }

            using (SKPaint subtitlePaint = new SKPaint
            {
                Color = new SKColor(164, 164, 164),
                IsAntialias = true
            })
            {
                canvas.DrawText(CreateText(rankingName, isMedium: true), 120, 100, subtitlePaint);
            }
        }

        private static SKTextBlob CreateText(string text, int fontSize = 24, bool isBold = false, bool isMedium = false)
        {
            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true
            };

            using var font = new SKFont(GetPoppinsTypeface(isBold, isMedium), fontSize);
            using var builder = new SKTextBlobBuilder();
            var buffer = builder.AllocateRun(font, text.Length, 0, 0);

            for (int i = 0; i < text.Length; i++)
            {
                buffer.Glyphs[i] = font.GetGlyphs(text[i].ToString())[0];
            }

            return builder.Build() ?? throw new InvalidOperationException("Failed to build SKTextBlob");
        }

        private static void DrawSection(SKCanvas canvas, string[,] strings)
        {
            using (SKPaint sectionPaint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
            })
            {
                canvas.DrawText(CreateText("# Ranking", 29, true), 20, 190, sectionPaint);
            }


            DrawStatsTable(canvas, strings, 20, 220, 1160);
        }

        private static void DrawStatsTable(SKCanvas canvas, string[,] data, int x, int y, int width)
        {
            int rowHeight = 60;
            int rows = data.GetLength(0);

            for (int i = 0; i < rows; i++)
            {
                using (SKPaint rowPaint = new SKPaint
                {
                    Color = new SKColor(47, 49, 54),
                    IsAntialias = true
                })
                {
                    SKRect rect = new SKRect(x, y + (i * (rowHeight + 10)), x + width, y + (i * (rowHeight + 10)) + rowHeight);
                    canvas.DrawRoundRect(rect, 10, 10, rowPaint);
                }

                SKRect rectRowPaint = new SKRect(x, y + (i * (rowHeight + 10)), x + 60, y + (i * (rowHeight + 10)) + rowHeight);
                using (SKPaint rowPaint = new SKPaint
                {
                    Color = new SKColor(24, 25, 27),
                    IsAntialias = true
                })
                {
                    canvas.DrawRoundRect(rectRowPaint, 10, 10, rowPaint);
                }

                using (SKPaint valuePaint = new SKPaint
                {
                    Color = SKColors.White,
                    IsAntialias = true,
                })
                using (SKPaint namePaint = new SKPaint
                {
                    Color = SKColors.White,
                    IsAntialias = true,
                })
                using (SKPaint rankPaint = new SKPaint
                {
                    Color = SKColors.White,
                    IsAntialias = true,
                })
                {
                    var text = (i + 1).ToString();

                    using (var font = new SKFont(GetPoppinsTypeface(), 22))
                    {
                        float textWidth = font.MeasureText(text);

                        SKFontMetrics metrics;
                        font.GetFontMetrics(out metrics);
                        float textHeight = metrics.Descent - metrics.Ascent;

                        float rectWidth = rectRowPaint.Width;
                        float rectHeight = rectRowPaint.Height;

                        float textX = x + (rectWidth - textWidth) / 2;
                        float textY = y + (i * (rowHeight + 10)) + (rectHeight - textHeight) / 2 - metrics.Ascent;

                        ///draw ranking
                        canvas.DrawText(CreateText(text, 22, true), textX, textY, rankPaint);

                        ///draw name
                        canvas.DrawText(CreateText(data[i, 0], 22), x + 80, textY, namePaint);

                        ///draw reason 
                        textWidth = font.MeasureText(data[i, 2]);
                        textX = x + (width - textWidth) / 2;
                        canvas.DrawText(CreateText(data[i, 2], 22, false), textX, textY, valuePaint);

                        ///draw point  
                        textWidth = font.MeasureText(data[i, 1]);
                        textX = x + (width - textWidth) - 20;
                        canvas.DrawText(CreateText(data[i, 1], 22, true), textX, textY, valuePaint);
                    }
                }
            }
        }

        private static void DrawFooter(SKCanvas canvas)
        {
            using (SKPaint footerPaint = new SKPaint
            {
                Color = SKColors.LightGray,
                IsAntialias = true,
            })
            {
                string text = "Powered by Porcupine";
                using (var font = new SKFont(GetPoppinsTypeface(), 20))
                {
                    float textWidth = font.MeasureText(text);
                    canvas.DrawText(CreateText("Powered by Porcupine", 20), 1180 - textWidth, Height - 20, footerPaint);
                }

                string dataFormatada = DateTime.Now.ToString("d 'of' MMMM 'of' yyyy 'at' HH:mm", new CultureInfo("en-US"));
                canvas.DrawText(CreateText($"Image generated in {dataFormatada}", 18), 20, Height - 20, footerPaint);
            }
        }

        private static SKTypeface GetPoppinsTypeface(bool isBold = false, bool isMedium = false)
        {
            try
            {
                string fontPath = isBold ? POPPINS_BOLD : (isMedium ? POPPINS_MEDIUM : POPPINS_REGULAR);

                if (File.Exists(fontPath))
                {
                    return SKTypeface.FromFile(fontPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Poppins font: {ex.Message}");
            }

            return SKTypeface.FromFamilyName(
                "Arial",
                isBold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright);
        }
    }
}
