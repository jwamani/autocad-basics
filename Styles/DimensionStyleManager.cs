using Autodesk.AutoCAD.DatabaseServices;

namespace TestDrawing.Styles
{
    public static class DimensionStyleManager
    {
        public static ObjectId GetOrCreate(
            Database database,
            Transaction transaction,
            string name)
        {
            DimStyleTable dimStyleTable = (DimStyleTable)transaction.GetObject(database.DimStyleTableId, OpenMode.ForRead);

            if (dimStyleTable.Has(name))
                return dimStyleTable[name];

            DimStyleTableRecord style = new DimStyleTableRecord()
            {
                Name = name
            };

            dimStyleTable.UpgradeOpen();

            ObjectId styleId = dimStyleTable.Add(style);
            transaction.AddNewlyCreatedDBObject(style, true);

            return styleId;
        }
    }
}
