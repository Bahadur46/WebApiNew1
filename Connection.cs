namespace WebApiNew
{
    public class Connection
    {
        private static string AttendanceDBConnection { get; set; }
        static Connection()
        {
            AttendanceDBConnection = "mongodb+srv://bahadur3028_db_user:HDTiC60Z7wBPxaey@clusterdhs.c0ai6pq.mongodb.net/";

        }

        public static string getAttendanceConnection()
        {
            return AttendanceDBConnection;
        }

    }
}