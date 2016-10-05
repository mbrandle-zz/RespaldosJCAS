using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace RespaldosJCAS
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Iniciando Proceso de Respaldo");

                Configuration configManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationCollection confCollection = configManager.AppSettings.Settings;

                String host = confCollection["host"].Value.ToString();
                String usuario = confCollection["usuario"].Value.ToString();
                String password = confCollection["password"].Value.ToString();
                String instacia = confCollection["instancia"].Value.ToString();
                String directorio = confCollection["directorios"].Value.ToString();
                String[] directorios = directorio.Split(';');

                DateTime Hoy = DateTime.Today;
                string fecha_actual = Hoy.ToString("dd-MM-yyyy");

                string constring = "server=" + host + ";user=" + usuario + ";pwd=" + password + ";database=" + instacia + "; pooling=false; convert zero datetime=True;";
                
                
                //string file = txtDireccion.Text + txtInstancia.Text + "_" + fecha_actual + ".sql";

                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            for (int i = 0; i < directorios.Length; i++)
                            {
                                Console.WriteLine("Iniciando respaldo en: " + directorios[i] + instacia + "_" + fecha_actual + ".sql ...");
                                mb.ExportToFile(directorios[i]+ instacia + "_" + fecha_actual + ".sql");
                                Console.WriteLine("Respaldo en: " + directorios[i] + instacia + "_" + fecha_actual + ".sql Terminado");
                            }
                            conn.Close();
                        }
                    }
                }
                loger("Respaldo Correcto.");
                Console.WriteLine("Proceso de Respaldo Terminado");
            }
            catch (Exception ex)
            {
                loger(ex.Message.ToString());
            }
        }

        static void loger(string msj)
        {
            try
            {
                string path = @"C:\\Respaldo.log";
                DateTime Hoy = DateTime.Now;
                string fecha_actual = Hoy.ToString("dd-MM-yyyy HH:mm:ss");
                if (!File.Exists(path))
                {
                    File.Create(path);
                    TextWriter tw = new StreamWriter(path);
                    tw.WriteLine("[" + fecha_actual + "] " + msj);
                    tw.Close();
                }
                else if (File.Exists(path))
                {
                    TextWriter tw = new StreamWriter(path, true);
                    tw.WriteLine("[" + fecha_actual + "] " + msj);
                    tw.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
