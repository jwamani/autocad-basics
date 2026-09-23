namespace TestDrawing.Drawing
{
    public static class DrawingConstants
    {
        // Sheet
        public const double SheetWidth = 3287;
        public const double SheetHeight = 1937;

        // Border
        public const double BorderStartWidth = 150;
        public const double BorderTaperWidth = 250;

        // Inner geometry
        public const double InnerOffset = 200;
        public const double InnerArcRadius = 300;

        // Circle / second arc
        public const double CircleRadius = 200;
        public const double Arc2Radius = 200;
        public const double Arc2CenterOffsetX = 600;
        public const double Arc2StartAngleDeg = 329;
        public const double Arc2EndAngleDeg = 117;

        // Text labels
        public const double LabelTextHeight = 62.5;
        public const double CircleLabelOffsetX = -250;
        public const double CircleLabelOffsetY = 300;
        public const double ArcLabelOffsetY = 250;

        // Dimensions
        public const double DimensionOffset = 300;

        // Layers
        public const string DimensionLayerName = "DIM50";

        // Text styles
        public const string TextStyleName = "Text 25";
        public const double TextStyleHeight = 62.5;
        public const double TextStyleWidthFactor = 0.7;

        public const string TextStyle50Name = "Text 50";
        public const double TextStyle50Height = 125;
        public const double TextStyle50WidthFactor = 0.7;

        // Dimension styles
        public const string DimStyleName = "Dim 50";
        public const double DimArrowSize = 2.5;

        // ACI color indices
        public const int AcByBlock = 0;
        public const int AcRed = 1;
        public const int AcYellow = 2;
        public const int AcGreen = 3;
        public const int AcCyan = 4;
        public const int AcBlue = 5;
        public const int AcMagenta = 6;
        public const int AcByLayer = 256;
    }
}
