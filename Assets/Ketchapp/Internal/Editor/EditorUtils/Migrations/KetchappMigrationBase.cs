using System;
namespace Ketchapp.Editor.Utils.Migrations
{
    public class KetchappMigrationBase
    {
        public virtual System.Version MigrationSDKVersion { get; }
        public virtual void ApplyMigration()
        {
        }

        public KetchappMigrationBase()
        {
        }
    }
}
