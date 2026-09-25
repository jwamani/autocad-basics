```csharp

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using TestDrawing.Layers;
using System;
using TestDrawing.Styles;


namespace TestDrawing.Drawing
{
    public static class TestDrawingBuilder
    {
        public static void Build(Database database)
        {
            double width = 3287;
            double height = 1937;

            // polyline
            Polyline rectangle = new Polyline()
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, 5),
            };

            rectangle.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
            rectangle.AddVertexAt(1, new Point2d(width, 0), 0, 0, 0);
            rectangle.AddVertexAt(2, new Point2d(width, height), 0, 0, 0);
            rectangle.AddVertexAt(3, new Point2d(0, height), 0, 0, 0);

            rectangle.SetStartWidthAt(0, 150);
            rectangle.SetEndWidthAt(0, 150);

            rectangle.SetStartWidthAt(2, 0);
            rectangle.SetEndWidthAt(2, 250);
            rectangle.Closed = true;

            Point3d bottomLeft = Point3d.Origin;
            Point3d bottomRight = new Point3d(width, 0, 0);
            Point3d topRight = new Point3d(width, height, 0);
            Point3d topLeft = new Point3d(0, height, 0);

            // inner lines
            double innerLineOffset = 200;
            double arcRadius = 300;

            Point3d vertex1 = new Point3d(topLeft.X + innerLineOffset, topLeft.Y - innerLineOffset, 0);
            Point3d vertex2 = new Point3d(topLeft.X + innerLineOffset, bottomLeft.Y + innerLineOffset, 0);
            Point3d vertex3 = new Point3d(bottomRight.X - innerLineOffset, bottomRight.Y + innerLineOffset, 0);
            Point3d vertex4 = new Point3d(bottomRight.X - innerLineOffset, topRight.Y - (innerLineOffset + arcRadius), 0);
            Point3d vertex5 = new Point3d(topRight.X - (innerLineOffset + arcRadius), topRight.Y - innerLineOffset, 0);

            Line line1 = new Line(vertex1, vertex2)
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, 4)
            };
            Line line2 = new Line(vertex2, vertex3)
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, 3)
            };
            Line line3 = new Line(vertex3, vertex4)
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, 2)
            };
            Line line4 = new Line(vertex1, vertex5)
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, 1)
            };

            Arc arc = new Arc(
                new Point3d(vertex5.X, vertex5.Y - arcRadius, 0),
                arcRadius,
                0,
                Math.PI / 2
            )
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, 6)
            };

            Point3d circleCenter = new Point3d(width / 2, height / 2, 0);

            Circle circle = new Circle(circleCenter, Vector3d.ZAxis, 200)
            {
                Color = Color.FromColorIndex(ColorMethod.ByLayer, 256)
            };

            Point3d arcCenter2 = new Point3d(circleCenter.X + 600, circleCenter.Y, 0);
            double degToRad = Math.PI / 180.0;
            Arc arc2 = new Arc(arcCenter2, 200, 329 * degToRad, 117 * degToRad)
            {
                Color = Color.FromColorIndex(ColorMethod.ByBlock, 0)
            };

            MText circleLabel = new MText()
            {
                Location = new Point3d(circleCenter.X - 250, circleCenter.Y + 300, 0),
                Contents = @"\LCircle",
                TextHeight = 62.5,
                Color = Color.FromColorIndex(ColorMethod.ByLayer, 256),
            };

            DBText arcLabel = new DBText()
            {
                Position = new Point3d(arcCenter2.X, arcCenter2.Y + 250, 0),
                Height = 62.5,
                TextString = "Arc",
                Color = Color.FromColorIndex(ColorMethod.ByColor, 4)
            };




            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord modelSpace = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // layers
                ObjectId DefaultLayerId = LayerManager.GetOrCreateLayer(database, transaction, "0", Color.FromRgb(255, 255, 255));
                ObjectId Dim50LayerId = LayerManager.GetOrCreateLayer(database, transaction, "DIM50", Color.FromColorIndex(ColorMethod.ByColor, 2)); // yellow

                // text styles
                ObjectId text25StyleId = TextStyleManager.GetOrCreate(database, transaction, "Text 25", height: 62.5, widthFactor: 0.7);

                // dimension styles
                ObjectId dim50StyleId = DimensionStyleManager.GetOrCreate(database, transaction, "Dim 50");

                RotatedDimension widthDimension = new RotatedDimension(
                    0,
                    topLeft,
                    topRight,
                    new Point3d(width / 2, -300, 0),
                    "",
                    dim50StyleId
                )
                {
                    LayerId = Dim50LayerId
                };

                RotatedDimension heightDim = new RotatedDimension(
                    Math.PI / 2,
                    bottomLeft,
                    new Point3d(0, height, 0),
                    new Point3d(-300, height / 2, 0),
                    "",
                    dim50StyleId
                )
                {
                    LayerId = Dim50LayerId
                };

                // assign layers
                rectangle.LayerId = DefaultLayerId;
                line1.LayerId = DefaultLayerId;
                line2.LayerId = DefaultLayerId;
                line3.LayerId = DefaultLayerId;
                line4.LayerId = DefaultLayerId;
                arc.LayerId = DefaultLayerId;
                circle.LayerId = DefaultLayerId;
                arc2.LayerId = DefaultLayerId;

                circleLabel.LayerId = DefaultLayerId;
                arcLabel.LayerId = DefaultLayerId;
                circleLabel.TextStyleId = text25StyleId;
                arcLabel.TextStyleId = text25StyleId;

                modelSpace.AppendEntity(rectangle);
                modelSpace.AppendEntity(line1);
                modelSpace.AppendEntity(line2);
                modelSpace.AppendEntity(line3);
                modelSpace.AppendEntity(line4);
                modelSpace.AppendEntity(arc);
                modelSpace.AppendEntity(circle);
                modelSpace.AppendEntity(arc2);

                modelSpace.AppendEntity(circleLabel);
                modelSpace.AppendEntity(arcLabel);

                modelSpace.AppendEntity(widthDimension);

                transaction.AddNewlyCreatedDBObject(rectangle, true);
                transaction.AddNewlyCreatedDBObject(line1, true);
                transaction.AddNewlyCreatedDBObject(line2, true);
                transaction.AddNewlyCreatedDBObject(line3, true);
                transaction.AddNewlyCreatedDBObject(line4, true);
                transaction.AddNewlyCreatedDBObject(arc, true);
                transaction.AddNewlyCreatedDBObject(circle, true);
                transaction.AddNewlyCreatedDBObject(arc2, true);
                transaction.AddNewlyCreatedDBObject(circleLabel, true);
                transaction.AddNewlyCreatedDBObject(arcLabel, true);
                transaction.AddNewlyCreatedDBObject(widthDimension, true);

                transaction.Commit();
            }

        }
    }
}```
