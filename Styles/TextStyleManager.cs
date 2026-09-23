using Autodesk.AutoCAD.DatabaseServices;

namespace TestDrawing.Styles
{
    public static class TextStyleManager
    {
        public static ObjectId GetOrCreate(
            Database database,
            Transaction transaction,
            string name,
            double widthFactor = 1.0,
            double height = 0.0
            )
        {
            TextStyleTable table =
                (TextStyleTable)transaction.GetObject(
                    database.TextStyleTableId,
                    OpenMode.ForRead
                );

            if (table.Has(name))
                return table[name];

            TextStyleTableRecord style =
                new TextStyleTableRecord
                {
                    Name = name,
                    FileName = "Arial.ttf",
                    TextSize = height,
                    XScale = widthFactor
                };

            table.UpgradeOpen();

            ObjectId styleId = table.Add(style);

            transaction.AddNewlyCreatedDBObject(
                style,
                true
            );

            return styleId;
        }
    }

}

