using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using TestDrawing.Layers;
using TestDrawing.Styles;
using System;

namespace TestDrawing.Drawing
{
    public static class TestDrawingBuilder
    {
        public static void Build(Database database)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord modelSpace = (BlockTableRecord)transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                ObjectId dimensionLayerId = LayerManager.GetOrCreateLayer(
                    database,
                    transaction,
                    DrawingConstants.DimensionLayerName,
                    Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcYellow));

                ObjectId leaderLayerId = LayerManager.GetOrCreateLayer(
                    database,
                    transaction,
                    DrawingConstants.LeaderLayerName,
                    Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcRed));
                ObjectId wallsLayerId = LayerManager.GetOrCreateLayer(
                    database,
                    transaction,
                    DrawingConstants.WallsLayerName,
                    Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcCyan));
                ObjectId hatchLayerId = LayerManager.GetOrCreateLayer(
                    database,
                    transaction,
                    DrawingConstants.HatchLayerName,
                    Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcCyan),
                    LineWeight.LineWeight020);

                ObjectId textStyleId = TextStyleManager.GetOrCreate(
                    database,
                    transaction,
                    DrawingConstants.TextStyleName,
                    widthFactor: DrawingConstants.TextStyleWidthFactor,
                    height: DrawingConstants.TextStyleHeight);

                ObjectId textStyle50Id = TextStyleManager.GetOrCreate(
                    database,
                    transaction,
                    DrawingConstants.TextStyle50Name,
                    widthFactor: DrawingConstants.TextStyle50WidthFactor,
                    height: DrawingConstants.TextStyle50Height);

                ObjectId dimStyleId = DimensionStyleManager.GetOrCreate(
                    database,
                    transaction,
                    DrawingConstants.DimStyleName,
                    textStyleId: textStyle50Id,
                    textHeight: DrawingConstants.TextStyle50Height,
                    arrowSize: DrawingConstants.DimArrowSize,
                    arrowBlockName: DrawingConstants.DimArrowBlock,
                    textColor: Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcCyan),
                    lineColor: Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcRed));

                CreateBorder(transaction, modelSpace);
                CreateInnerLines(transaction, modelSpace, out Point3d vertex1, out Point3d vertex3, out Point3d vertex5);
                CreateArcsAndCircles(transaction, modelSpace, vertex5, out Circle circle, out Arc arc2);
                CreateLabels(transaction, modelSpace, textStyleId);
                CreateDimensions(transaction, modelSpace, dimensionLayerId, dimStyleId, vertex1, vertex3);

                transaction.Commit();
            }
        }

        private static void CreateBorder(Transaction transaction, BlockTableRecord modelSpace)
        {
            double width = DrawingConstants.SheetWidth;
            double height = DrawingConstants.SheetHeight;

            Polyline border = new Polyline()
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcBlue),
            };

            border.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
            border.AddVertexAt(1, new Point2d(width, 0), 0, 0, 0);
            border.AddVertexAt(2, new Point2d(width, height), 0, 0, 0);
            border.AddVertexAt(3, new Point2d(0, height), 0, 0, 0);

            border.SetStartWidthAt(0, DrawingConstants.BorderStartWidth);
            border.SetEndWidthAt(0, DrawingConstants.BorderStartWidth);
            border.SetStartWidthAt(2, 0);
            border.SetEndWidthAt(2, DrawingConstants.BorderTaperWidth);
            border.Closed = true;

            EntityAppender.Append(transaction, modelSpace, border);
        }

        private static void CreateInnerLines(Transaction transaction, BlockTableRecord modelSpace, out Point3d vertex1, out Point3d vertex3, out Point3d vertex5)
        {
            double width = DrawingConstants.SheetWidth;
            double height = DrawingConstants.SheetHeight;
            double innerLineOffset = DrawingConstants.InnerOffset;
            double arcRadius = DrawingConstants.InnerArcRadius;

            Point3d bottomLeft = Point3d.Origin;
            Point3d bottomRight = new Point3d(width, 0, 0);
            Point3d topRight = new Point3d(width, height, 0);
            Point3d topLeft = new Point3d(0, height, 0);

            vertex1 = new Point3d(topLeft.X + innerLineOffset, topLeft.Y - innerLineOffset, 0);
            Point3d vertex2 = new Point3d(topLeft.X + innerLineOffset, bottomLeft.Y + innerLineOffset, 0);
            vertex3 = new Point3d(bottomRight.X - innerLineOffset, bottomRight.Y + innerLineOffset, 0);
            vertex5 = new Point3d(topRight.X - (innerLineOffset + arcRadius), topRight.Y - innerLineOffset, 0);

            Line line1 = new Line(vertex1, vertex2)
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcCyan)
            };
            Line line2 = new Line(vertex2, vertex3)
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcGreen)
            };
            Line line3 = new Line(vertex3, new Point3d(topRight.X - innerLineOffset, topRight.Y - (innerLineOffset + arcRadius), 0))
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcYellow)
            };
            Line line4 = new Line(vertex1, vertex5)
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcRed)
            };

            EntityAppender.Append(transaction, modelSpace, line1);
            EntityAppender.Append(transaction, modelSpace, line2);
            EntityAppender.Append(transaction, modelSpace, line3);
            EntityAppender.Append(transaction, modelSpace, line4);
        }

        private static void CreateArcsAndCircles(
            Transaction transaction,
            BlockTableRecord modelSpace,
            Point3d vertex5,
            out Circle circle,
            out Arc arc2)
        {
            double width = DrawingConstants.SheetWidth;
            double height = DrawingConstants.SheetHeight;
            double arcRadius = DrawingConstants.InnerArcRadius;

            Arc cornerArc = new Arc(
                new Point3d(vertex5.X, vertex5.Y - arcRadius, 0),
                arcRadius,
                0,
                Math.PI / 2
            )
            {
                Color = Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcMagenta)
            };

            Point3d circleCenter = new Point3d(width / 2, height / 2, 0);

            circle = new Circle(circleCenter, Vector3d.ZAxis, DrawingConstants.CircleRadius)
            {
                Color = Color.FromColorIndex(ColorMethod.ByLayer, DrawingConstants.AcByLayer)
            };

            Point3d arc2Center = new Point3d(circleCenter.X + DrawingConstants.Arc2CenterOffsetX, circleCenter.Y, 0);
            double degToRad = Math.PI / 180.0;
            arc2 = new Arc(
                arc2Center,
                DrawingConstants.Arc2Radius,
                DrawingConstants.Arc2StartAngleDeg * degToRad,
                DrawingConstants.Arc2EndAngleDeg * degToRad
            )
            {
                Color = Color.FromColorIndex(ColorMethod.ByBlock, DrawingConstants.AcByBlock)
            };

            EntityAppender.Append(transaction, modelSpace, cornerArc);
            EntityAppender.Append(transaction, modelSpace, circle);
            EntityAppender.Append(transaction, modelSpace, arc2);
        }

        private static void CreateLabels(Transaction transaction, BlockTableRecord modelSpace, ObjectId textStyleId)
        {
            double width = DrawingConstants.SheetWidth;
            double height = DrawingConstants.SheetHeight;

            Point3d circleCenter = new Point3d(width / 2, height / 2, 0);
            Point3d arc2Center = new Point3d(circleCenter.X + DrawingConstants.Arc2CenterOffsetX, circleCenter.Y, 0);

            MText circleLabel = new MText()
            {
                Location = new Point3d(
                    circleCenter.X + DrawingConstants.CircleLabelOffsetX,
                    circleCenter.Y + DrawingConstants.CircleLabelOffsetY,
                    0),
                Contents = @"\LCircle",
                TextHeight = DrawingConstants.LabelTextHeight,
                Color = Color.FromColorIndex(ColorMethod.ByLayer, DrawingConstants.AcByLayer),
            };

            DBText arcLabel = new DBText()
            {
                Position = new Point3d(arc2Center.X, arc2Center.Y + DrawingConstants.ArcLabelOffsetY, 0),
                Height = DrawingConstants.LabelTextHeight,
                TextString = "Arc",
                Color = Color.FromColorIndex(ColorMethod.ByColor, DrawingConstants.AcCyan)
            };

            EntityAppender.Append(transaction, modelSpace, circleLabel, ObjectId.Null, textStyleId);
            EntityAppender.Append(transaction, modelSpace, arcLabel, ObjectId.Null, textStyleId);
        }

        private static void CreateDimensions(
            Transaction transaction,
            BlockTableRecord modelSpace,
            ObjectId dimensionLayerId,
            ObjectId dimStyleId,
            Point3d vertex1,
            Point3d vertex3)
        {
            double width = DrawingConstants.SheetWidth;
            double height = DrawingConstants.SheetHeight;

            Point3d bottomLeft = Point3d.Origin;
            Point3d topLeft = new Point3d(0, height, 0);
            Point3d topRight = new Point3d(width, height, 0);

            RotatedDimension widthDimension = new RotatedDimension(
                0,
                topLeft,
                topRight,
                new Point3d(width / 2, height + DrawingConstants.DimensionOffset, 0),
                "",
                dimStyleId
            );

            RotatedDimension heightDimension = new RotatedDimension(
                Math.PI / 2,
                bottomLeft,
                topLeft,
                new Point3d(-DrawingConstants.DimensionOffset, height / 2, 0),
                "",
                dimStyleId
            )
            {
                TextRotation = Math.PI / 2,
                Dimtad = 1,
                UsingDefaultTextPosition = false,
                Dimtmove = 2,
                TextPosition = new Point3d(-350, height / 2, 0)
            };

            AlignedDimension diagonalDimension = new AlignedDimension(
                vertex1,
                vertex3,
                new Point3d(
                    (vertex1.X + vertex3.X) / 2,
                    (vertex1.Y + vertex3.Y) / 2,
                    0),
                "",
                dimStyleId
            )
            {
                Dimgap = 5,
                Dimjust = 0,
                TextRotation = 330 * (Math.PI / 180.0),
                UsingDefaultTextPosition = false,
                Dimtmove = 2,
                TextPosition = new Point3d(
                    (vertex1.X + vertex3.X) / 3,
                    vertex1.Y - 300,
                    0)
            };


            RotatedDimension miniDimension = new RotatedDimension(
                Math.PI / 2,
                new Point3d(
                    vertex3.X - 1300,
                    vertex3.Y - 200,
                    0
                ),
                new Point3d(
                    vertex3.X - 1300,
                    vertex3.Y,
                    0
                ),
                new Point3d(
                    vertex3.X - 1200,
                    vertex3.Y / 2,
                    0
                ),
                "",
                dimStyleId
            )
            {
                TextRotation = 0,
            };

            EntityAppender.Append(transaction, modelSpace, widthDimension, dimensionLayerId);
            EntityAppender.Append(transaction, modelSpace, heightDimension, dimensionLayerId);
            EntityAppender.Append(transaction, modelSpace, diagonalDimension, dimensionLayerId);
            EntityAppender.Append(transaction, modelSpace, miniDimension, dimensionLayerId);
        }
    }
}
