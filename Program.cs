using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;

namespace RefreshDistrictDb
{
    class Program
    {

        static string connectionString;
        static string csvFileName;

        static void Main()
        {
            connectionString = ConfigurationManager.AppSettings["connectionString"];
            csvFileName = ConfigurationManager.AppSettings["csvFileName"];


            DataTable dt = LoadCSV();

            using MySqlConnection connection = new(connectionString);
            connection.Open();
            ClearTable(connection);
            LoadTable(dt, connection);
        }

        /// <summary>
        /// Loads a District database-ready DataTable from the CSV file at csvFilePath
        /// </summary>
        /// <returns>The DataTable</returns>
        private static DataTable LoadCSV()
        {
            var table = new DataTable();
            var records = new List<Record>();


            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                BadDataFound = null,
                MissingFieldFound= null,
            };

            var typeConverterFactory = new TypeConverterCache();
            var mdtc = new MyDateTimeConverter();
            typeConverterFactory.AddConverter<DateTime>(mdtc);
            
            string tmp = File.ReadAllText(csvFileName);

            using (var reader = new StreamReader(csvFileName))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<RecordMap>();
                csv.Context.Configuration.PrepareHeaderForMatch = h => string.IsNullOrWhiteSpace(h.Header) ? Guid.NewGuid().ToString("N") : h.Header;
                csv.Read();
                csv.ReadHeader();
                records = csv.GetRecords<Record>().ToList();
            }

            foreach (var prop in typeof(Record).GetProperties())
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var record in records)
            {
                var row = table.NewRow();

                foreach (var prop in typeof(Record).GetProperties())
                {
                    row[prop.Name] = prop.GetValue(record) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// Deletes and recreates empty, the table in the MySql database at the provided connection.
        /// </summary>
        /// <param name="connection"></param>
        private static void ClearTable(MySqlConnection connection)
        {
            // Delete the table if it exists
            string dropTableQuery = "DROP TABLE IF EXISTS membership;";
            using (MySqlCommand cmd = new(dropTableQuery, connection))
            {
                cmd.ExecuteNonQuery();
            }

            // Recreate the table
            string createTableQuery = @"
                    CREATE TABLE membership (
                        membership_type VARCHAR(255),
                        title VARCHAR(255),
                        first_name VARCHAR(255),
                        middle_name VARCHAR(255),
                        last_name VARCHAR(255),
                        nickname VARCHAR(255),
                        suffix VARCHAR(255),
                        username VARCHAR(255),
                        gender VARCHAR(255),
                        club VARCHAR(255),
                        club_position VARCHAR(255),
                        district_defined_club_position VARCHAR(255),
                        district_position VARCHAR(255),
                        email VARCHAR(255),
                        alternate_email VARCHAR(255),
                        preferred_address VARCHAR(255),
                        preferred_address1 VARCHAR(255),
                        preferred_address2 VARCHAR(255),
                        preferred_city VARCHAR(255),
                        preferred_state VARCHAR(255),
                        preferred_zip_code VARCHAR(255),
                        preferred_country VARCHAR(255),
                        address_line_1 VARCHAR(255),
                        address_line_2 VARCHAR(255),
                        city VARCHAR(255),
                        state_province VARCHAR(255),
                        zip_postal_code VARCHAR(255),
                        country VARCHAR(255),
                        preferred_phone_type VARCHAR(255),
                        preferred_phone VARCHAR(255),
                        home_phone VARCHAR(255),
                        home_fax VARCHAR(255),
                        pager VARCHAR(255),
                        cell VARCHAR(255),
                        business_phone VARCHAR(255),
                        business_fax VARCHAR(255),
                        company_name VARCHAR(255),
                        position_title VARCHAR(255),
                        classification VARCHAR(255),
                        business_address1 VARCHAR(255),
                        business_address2 VARCHAR(255),
                        business_city VARCHAR(255),
                        business_state_province VARCHAR(255),
                        business_zip_postal_code VARCHAR(255),
                        business_country VARCHAR(255),
                        date_of_birth DATE,
                        spouse_partner_first_name VARCHAR(255),
                        spouse_partner_last_name VARCHAR(255),
                        spouse_partner_nick_name VARCHAR(255),
                        spouse_partner_date_of_birth DATE,
                        anniversary DATE,
                        member_no VARCHAR(255),
                        website_url VARCHAR(255),
                        membership VARCHAR(255),
                        office VARCHAR(255),
                        sponsor VARCHAR(255),
                        date_joined_rotary DATE,
                        date_joined_club DATE,
                        years_of_service_rotary INT,
                        years_of_service_club INT,
                        personal_url VARCHAR(255),
                        termination_date DATE,
                        reason_for_termination VARCHAR(255),
                        date_created DATE,
                        date_modified DATE,
                        last_login DATE,
                        club_mailing_address VARCHAR(255),
                        club_phone VARCHAR(255),
                        club_email VARCHAR(255),
                        club_website VARCHAR(255),
                        club_fax VARCHAR(255),
                        badge_no VARCHAR(255),
                        club_area VARCHAR(255),
                        club_assistant_area_governor VARCHAR(255)
                    );";

            using (MySqlCommand cmd = new(createTableQuery, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Loads the MySql database at the provided connection with the data in the provided DataTable.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="connection"></param>
        private static void LoadTable(DataTable dt, MySqlConnection connection)
        {
            try
            {
                // Begin a transaction for bulk insert
                using MySqlTransaction transaction = connection.BeginTransaction();
                StringBuilder insert = new();
                dt.Columns.Cast<DataColumn>().ToList().ForEach(column => insert.Append(column.ColumnName + ",\n"));
                string sqlColumns = insert.ToString()[..(insert.Length - 2)];

                StringBuilder values = new();
                dt.Columns.Cast<DataColumn>().ToList().ForEach(column => values.Append("@" + column.ColumnName + ",\n"));
                string sqlValues = values.ToString()[..(values.Length - 2)];


                // MySQL command for inserting a row
                string insertCommand = $"INSERT INTO membership ( {sqlColumns} ) VALUES ( {sqlValues} )";

                // Loop through each row of the DataTable
                foreach (DataRow row in dt.Rows)
                {
                    using MySqlCommand cmd = new(insertCommand, connection, transaction);
                    // Add parameters for each column in the row
                    foreach (DataColumn column in dt.Columns)
                    {
                        Object val = row[column] ?? DBNull.Value;
                        if ((column.DataType.FullName == "System.DateTime" && val.Equals(DateTime.MinValue))
                        || (column.DataType.FullName == "System.Int32" && val.Equals(int.MinValue)))
                        {
                            val = DBNull.Value;
                        }
                        cmd.Parameters.AddWithValue("@" + column.ColumnName, val ?? @"''");
                    }

                    // Execute the insert command
                    cmd.ExecuteNonQuery();
                }

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Handle or log any errors that occurred during the bulk insert
                Console.Write("An error occurred: " + ex.Message);
                Console.Write(ex.StackTrace);
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }

    }
}
