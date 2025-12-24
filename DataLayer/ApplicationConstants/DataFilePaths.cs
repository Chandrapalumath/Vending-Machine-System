namespace DataLayer.ApplicationConstant
{
    internal static class DataFilePaths
    {
        public static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        public static readonly string UsersFile = Path.Combine(DataFolder, "UserInfo.txt");
        public static readonly string ItemsFile = Path.Combine(DataFolder, "Items.txt");
        public static readonly string TransactionsFile = Path.Combine(DataFolder, "Transactions.txt");
        public static readonly string AdminFile = Path.Combine(DataFolder, "Admin.txt");
    }
}
