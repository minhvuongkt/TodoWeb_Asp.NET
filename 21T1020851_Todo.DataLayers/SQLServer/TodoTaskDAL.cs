using _21T1020851_Todo.DomainModels;
using Dapper;
using System.Data;
using System.Globalization;

namespace _21T1020851_Todo.DataLayers.SQLServer
{
    public class TodoTaskDAL : _BaseDAL, ICommonDAL<TodoTask>
    {
        public TodoTaskDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(TodoTask item)
        {
            int id = 0;
            using (var conn = OpenConnection())
            {
                //string sql = @"insert into TodoTasks (TaskTitle, TaskDescription, IsFinished, TimeStart, TimeEnd, UserID) 
                //              values(@TaskTitle, @TaskDescription, @IsFinished, @TimeStart, @TimeEnd, @UserID);
                //              select scope_identity(); ";
                var param = new DynamicParameters();
                param.Add("@UserID", item.UserID);
                param.Add("@TaskTitle", item.TaskTitle);
                param.Add("@TaskDescription", item.TaskDescription);
                param.Add("@IsFinished", item.IsFinished);
                param.Add("@TimeStart", item.TimeStart);
                param.Add("@TimeEnd", item.TimeEnd);
                //id = conn.Query<int>(sql, param: param, commandType: CommandType.Text).FirstOrDefault();
                id = conn.ExecuteScalar<int>("sp_AddTodoTask", param: param, commandType: CommandType.StoredProcedure);
                conn.Close();
                return id;
            }
        }
        public int Count(string searchValue = "", string timeSearch = "", int UserID = 0)
        {
            int id = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"SELECT count(*) FROM TodoTasks 
                                       WHERE  UserID = @UserID
                                      AND (@searchValue IS NULL OR TaskTitle LIKE '%' + @searchValue + '%' OR TaskDescription LIKE '%' + @searchValue + '%')
                                      AND (@timeStart IS NULL OR TimeStart >= @timeStart AND TimeEnd <= @timeEnd)
                                            ";

                DateTime? timeStart = null;
                DateTime? timeEnd = null;
                if (timeSearch != "")
                {
                    var times = timeSearch.Split(" - ");
                    timeStart = DateTime.ParseExact(times[0], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    timeEnd = DateTime.ParseExact(times[1], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                var param = new DynamicParameters();
                param.Add("@UserID", UserID);
                param.Add("@SearchValue", searchValue);
                param.Add("@timeStart", timeStart);
                param.Add("@timeEnd", timeEnd);
                id = conn.Query<int>(sql, param: param, commandType: CommandType.Text).FirstOrDefault();
                conn.Close();
                return id;
            }
        }
        public int Count1(string searchValue = "", string timeSearch = "", int UserID = 0)
        {
            int id = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"SELECT count(*) FROM TodoTasks 
                                               WHERE  UserID = @UserID
                                              AND (@searchValue IS NULL OR TaskTitle LIKE '%' + @searchValue + '%' OR TaskDescription LIKE '%' + @searchValue + '%')
                                              AND (@timeStart = @timeEnd OR TimeStart IS NULL OR TimeEnd IS NULL OR (TimeStart >= @timeStart AND TimeEnd <= @timeEnd))
                                                    ";

                var date = DateTime.Now;
                var timeStart = date.ToString("dd-MM-yyyy");
                var timeEnd = date.ToString("dd-MM-yyyy");
                if (timeSearch != "")
                {
                    var times = timeSearch.Split(" - ");
                    timeStart = times[0];
                    timeEnd = times[1];
                }
                var param = new DynamicParameters();
                param.Add("@UserID", UserID);
                param.Add("@SearchValue", searchValue);
                param.Add("@timeStart", timeStart);
                param.Add("@timeEnd", timeEnd);
                id = conn.Query<int>(sql, param: param, commandType: CommandType.Text).FirstOrDefault();
                //id = conn.ExecuteScalar<int>("sp_CountTodoTask", param: param, commandType: CommandType.StoredProcedure);
                conn.Close();
                return id;
            }
        }
        public bool Delete(int id)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                //string sql = @"delete from TodoTasks where TaskID = @TaskID;";
                var param = new DynamicParameters();
                param.Add("@TaskID", id);
                //result = conn.Execute(sql: sql, param: param, commandType: CommandType.Text) > 0;
                result = conn.Execute("sp_DeleteTodoTask", param: param, commandType: CommandType.StoredProcedure) > 0;
                conn.Close();
            }
            return result;
        }
        public TodoTask? Get(int id)
        {
            var todo = new TodoTask();
            using (var conn = OpenConnection())
            {
                //var sql = "select * from TodoTasks where TaskID = @TaskID;";
                var param = new DynamicParameters();
                param.Add("@TaskID", id);
                //todo = conn.QueryFirstOrDefault<TodoTask>(sql: sql, param: param, commandType: CommandType.Text);
                todo = conn.QueryFirstOrDefault<TodoTask>("sp_GetTodoTask", param: param, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return todo;
        }
        public IList<TodoTask> List(int page = 1, int pageSize = 0, string searchValue = "", string timeSearch = "", int UserID = 0, int viewPage = 1)
        {
            var listTodos = new List<TodoTask>();
            using (var conn = OpenConnection())
            {
                var date = DateTime.Now;
                DateTime? timeStart = null;
                DateTime? timeEnd = null;

                if (timeSearch != "")
                {
                    var times = timeSearch.Split(" - ");
                    timeStart = DateTime.ParseExact(times[0], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    timeEnd = DateTime.ParseExact(times[1], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                string viewPages = viewPage switch
                {
                    1 => "CAST(TaskTitle AS NVARCHAR(MAX))",
                    2 => "CAST(TaskTitle AS NVARCHAR(MAX)) DESC",
                    3 => "CAST(TaskID AS NVARCHAR(MAX))",
                    4 => "CAST(TaskID AS NVARCHAR(MAX)) DESC",
                    _ => "CAST(TaskTitle AS NVARCHAR(MAX))"
                };

                string sql = $@" SELECT *
                                FROM (
                                    SELECT *, 
                                           ROW_NUMBER() OVER (ORDER BY {viewPages}) AS RowNumber
                                    FROM TodoTasks
                                    WHERE UserID = @UserID
                                          AND (@searchValue IS NULL OR TaskTitle LIKE CONCAT('%', @searchValue, '%') OR TaskDescription LIKE CONCAT('%', @searchValue, '%'))
                                          AND (@timeStart = @timeEnd OR TimeStart IS NULL OR TimeEnd IS NULL OR (TimeStart >= @timeStart AND TimeEnd <= @timeEnd))
                                ) AS t
                                WHERE (@pageSize = 0) OR (RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
                                ORDER BY RowNumber;";

                var param = new DynamicParameters();
                param.Add("@UserID", UserID);
                param.Add("@searchValue", searchValue);
                param.Add("@page", page);
                param.Add("@pageSize", pageSize);
                param.Add("@viewPage", viewPage);
                param.Add("@timeStart", timeStart);
                param.Add("@timeEnd", timeEnd);


                listTodos = conn.Query<TodoTask>(sql: sql, param: param, commandType: CommandType.Text).ToList();
                //listTodos = conn.Query<TodoTask>("sp_GetTodoTasks", param: param, commandType: CommandType.StoredProcedure).ToList();
                conn.Close();
                return listTodos;
            }
        }
        public bool Update(TodoTask item)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                /*var sql = @"update TodoTasks set TaskTitle = @TaskTitle, TaskDescription = @TaskDescription, 
                                        IsFinished = @IsFinished, TimeStart = @TimeStart, TimeEnd = @TimeEnd
                                        where TaskID = @TaskID;";*/
                var param = new DynamicParameters();
                param.Add("@TaskID", item.TaskID);
                param.Add("@TaskTitle", item.TaskTitle);
                param.Add("@TaskDescription", item.TaskDescription);
                param.Add("@IsFinished", item.IsFinished);
                param.Add("@TimeStart", item.TimeStart);
                param.Add("@TimeEnd", item.TimeEnd);
                result = conn.Execute("sp_UpdateTodoTask", param: param, commandType: CommandType.StoredProcedure) > 0;
                conn.Close();
            }
            return result;
        }
        public bool UpdateOther(string taskID, string isFinished)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                bool isFinish = isFinished == "true" ? true : false;
                var param = new DynamicParameters();
                param.Add("@TaskID", int.Parse(taskID));
                param.Add("@IsFinished", isFinish);
                result = conn.Execute("sp_UpdateTodoTaskIsFinished", param: param, commandType: CommandType.StoredProcedure) > 0;
                conn.Close();
            }
            return result;
        }

        #region Not Use
        public bool InUsed(int id)
        {
            return false;
        }

        public TodoTask? GetOther(string item1, string item2)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
