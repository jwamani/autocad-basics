using Autodesk.AutoCAD.DatabaseServices;

namespace TestDrawing.Drawing
{
    public static class EntityAppender
    {
        public static ObjectId Append(
            Transaction transaction,
            BlockTableRecord modelSpace,
            Entity entity)
        {
            return Append(transaction, modelSpace, entity, ObjectId.Null, ObjectId.Null);
        }

        public static ObjectId Append(
            Transaction transaction,
            BlockTableRecord modelSpace,
            Entity entity,
            ObjectId layerId)
        {
            return Append(transaction, modelSpace, entity, layerId, ObjectId.Null);
        }

        public static ObjectId Append(
            Transaction transaction,
            BlockTableRecord modelSpace,
            Entity entity,
            ObjectId layerId,
            ObjectId textStyleId)
        {
            if (!layerId.IsNull)
                entity.LayerId = layerId;

            if (!textStyleId.IsNull)
            {
                if (entity is DBText dbText)
                    dbText.TextStyleId = textStyleId;
                else if (entity is MText mText)
                    mText.TextStyleId = textStyleId;
            }

            modelSpace.AppendEntity(entity);
            transaction.AddNewlyCreatedDBObject(entity, true);
            return entity.ObjectId;
        }
    }
}