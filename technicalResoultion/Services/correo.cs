using Microsoft.Data.SqlClient;

namespace technicalResoultion.Services
{
    public class correo
    {
        private IConfiguration _configuration;

        public correo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void enviar(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                string connectionString = _configuration.GetSection("ConnectionStrings").GetSection("TechResConnection").Value;

                string sqlQuery = "exec msdb.dbo.sp_send_dbmail " +
                    "             @profile_name = 'SQLMail_CATOLICA'," +
                    "             @recipients = @par_destinatarios, " +
                    "             @subject = @par_asunto, " +
                    "             @body = @par_mensaje ";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@par_destinatarios", destinatario);
                        command.Parameters.AddWithValue("@par_asunto", asunto);
                        command.Parameters.AddWithValue("@par_mensaje", cuerpo);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception message
                Console.WriteLine("Error: ");
                Console.WriteLine(ex.Message);

                // Log the stack trace for more detailed information
                Console.WriteLine(ex.StackTrace);

                // Optionally, log the exception to a file or database
                // File.AppendAllText("path_to_log_file.txt", ex.ToString());
            }
        }
    }
}
