using System.Data;
using System.IO;
using System.Text.Json;
using Npgsql;

namespace TaskManagerADO.DAL
{
    public static class Database
    {
        private static readonly string _cs;

        static Database()
        {
            var json = File.ReadAllText("appsettings.json");
            var cfg = JsonSerializer.Deserialize<AppConfig>(json)!;
            _cs = cfg.ConnectionStrings.Default;
        }

        private static NpgsqlConnection Open()
        {
            var con = new NpgsqlConnection(_cs);
            con.Open();
            return con;
        }

        // SELECT
        public static DataTable GetTasks()
        {
            using var con = Open();
            using var da = new NpgsqlDataAdapter(
                @"SELECT id, title, description, created_at, completed
                  FROM tasks
                  ORDER BY completed, created_at DESC", con);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // INSERT
        public static void AddTask(string title, string? description)
        {
            using var con = Open();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO tasks(title, description) VALUES(@t, @d)", con);
            cmd.Parameters.AddWithValue("@t", title);
            cmd.Parameters.AddWithValue("@d", (object?)description ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        // UPDATE (переключить completed)
        public static void ToggleCompleted(int id)
        {
            using var con = Open();
            using var cmd = new NpgsqlCommand(
                "UPDATE tasks SET completed = NOT completed WHERE id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // DELETE
        public static void DeleteTask(int id)
        {
            using var con = Open();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM tasks WHERE id=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }

    // ↓ оставь как было
    public class AppConfig
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
    }

    public class ConnectionStrings
    {
        public string Default { get; set; } = "";
    }
}
