using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Directum2RxDocumentTransfer.Utils
{
    public class FormDataList
    {
        private int offset;
        private int limit;

        public FormDataList(int offset, int limit)
        {
            this.offset = offset;
            this.limit = limit;
        }

        public List<DTO.DataListEntity> GetDataList()
        {
            var entityList = new List<DTO.DataListEntity>();

            using (var connection = SQL.SqlHandler.CreateNewDirectumConnection())
            {
                connection.Open();
                var commandText = string.Format(SQL.SqlCommands.FormListCommand, offset, limit);
                Logger.Debug($"GetDataList. Query: {commandText}");

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandTimeout = 86400;
                    using (SqlDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            entityList.Add(new DTO.DataListEntity
                            {
                                DocumentId = reader.GetInt32(0),
                                TaskID = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                                Subject = reader.IsDBNull(4) ? null : reader.GetString(4)
                            });
                }
            }

            return entityList;
        }
    }
}
