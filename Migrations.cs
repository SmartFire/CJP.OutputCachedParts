using Orchard.Data.Migration;

namespace CJP.OutputCachedParts 
{
    public class Migrations : DataMigrationImpl 
    {
        public int Create() {
            SchemaBuilder
                .CreateTable("CacheKeyRecord",
                    table => table
                        .Column<int>("Id", column => column.PrimaryKey().Identity())
                        .Column<int>("ContentId")
                        .Column<string>("CacheKey", c => c.WithLength(2048)));

            return 1;
        }

        public int UpdateFrom1() {
            SchemaBuilder.AlterTable("CacheKeyRecord",
                table => table.AddColumn<string>("PartName"));

            return 2;
        }
    }
}