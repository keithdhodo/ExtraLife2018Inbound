using ExtraLife2018InboundETL.Entities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraLife2018InboundETL.Extensions
{
    public static class SqlExtensions<T>
    {
        private static IDictionary<int, Prize> PrizeCache;

        // https://stackoverflow.com/questions/18901545/return-result-from-select-query-in-stored-procedure-to-a-list
        public static async Task<IList<ParticipantTableEntity>> GetParticipantsFromSqlAsync(string sqlConnectionString)
        {
            var testList = new List<ParticipantTableEntity>();

            using (var SqlConnection = new SqlConnection(sqlConnectionString))
            {
                var command = new SqlCommand("GetParticipants", SqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                await SqlConnection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var test = new ParticipantTableEntity();
                    //test.ID = int.Parse(reader["ID"].ToString());
                    //test.Name = reader["Name"].ToString();
                    testList.Add(test);
                }

                SqlConnection.Close();
            }

            return testList;
        }

        public static async Task<IList<Prize>> GetWinnersFromSqlAsync(string sqlConnectionString)
        {
            var prizes = new List<Prize>();

            var maxCacheDate = (PrizeCache == null || PrizeCache.Count == 0) ? DateTime.MinValue : PrizeCache.Max(p => p.Value?.DateAddedToCache);

            if (maxCacheDate < DateTime.UtcNow.AddMinutes(-1))
            {
                PrizeCache = new ConcurrentDictionary<int, Prize>();

                using (var SqlConnection = new SqlConnection(sqlConnectionString))
                {
                    var command = new SqlCommand("GetWinners", SqlConnection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    await SqlConnection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();
                    var jsonResult = new StringBuilder();

                    while (await reader.ReadAsync())
                    {
                        jsonResult.Append(reader.GetValue(0).ToString());
                    }

                    prizes = JsonConvert.DeserializeObject<List<Prize>>(jsonResult.ToString());

                    SqlConnection.Close();
                }

                prizes.Select(p =>
                { p.SetDateAddedToCache(); return p; }
                ).ToList();

                foreach (var prize in prizes)
                {
                    PrizeCache.Add(prize.PrizeId, prize);
                }
            }
            else
            {
                foreach (var prize in PrizeCache.Values)
                {
                    prizes.Add(prize);
                }
            }

            return prizes;
        }

        public static async Task<bool> WriteParticipantsToSqlAsync(IList<ParticipantTableEntity> participants, string sqlConnectionString)
        {
            bool isSuccess = false;

            try
            {
                using (var SqlConnection = new SqlConnection(sqlConnectionString))
                {
                    await SqlConnection.OpenAsync();

                    foreach (var participant in participants)
                    {
                        using (var command = new SqlCommand("InsertParticipant", SqlConnection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.CommandTimeout = 90;

                            command.Parameters.Add("@ParticipantId", System.Data.SqlDbType.UniqueIdentifier);
                            command.Parameters["@ParticipantId"].Value = new Guid(participant.RowKey);

                            command.Parameters.Add("@ExtraLifeParticipantId", System.Data.SqlDbType.BigInt);
                            command.Parameters["@ExtraLifeParticipantId"].Value = participant.ParticipantId;

                            command.Parameters.Add("@ParticipantName", System.Data.SqlDbType.NVarChar);
                            command.Parameters["@ParticipantName"].Value = participant.DisplayName ?? string.Empty;

                            command.Parameters.Add("@TeamId", System.Data.SqlDbType.BigInt);
                            command.Parameters["@TeamId"].Value = participant.TeamId;

                            command.Parameters.Add("@TeamName", System.Data.SqlDbType.NVarChar);
                            command.Parameters["@TeamName"].Value = participant.TeamName ?? string.Empty;

                            command.Parameters.Add("@EventId", System.Data.SqlDbType.BigInt);
                            command.Parameters["@EventId"].Value = participant.EventId;

                            command.Parameters.Add("@EventName", System.Data.SqlDbType.NVarChar);
                            command.Parameters["@EventName"].Value = participant.EventName ?? string.Empty;

                            command.Parameters.Add("@IsTeamCaptain", System.Data.SqlDbType.Bit);
                            command.Parameters["@IsTeamCaptain"].Value = participant.IsTeamCaptain;

                            command.Parameters.Add("@AvatarImageURL", System.Data.SqlDbType.NVarChar);
                            command.Parameters["@AvatarImageURL"].Value = participant.AvatarImageURL ?? string.Empty;

                            command.Parameters.Add("@FundraisingGoal", System.Data.SqlDbType.Decimal);
                            command.Parameters["@FundraisingGoal"].Value = Convert.ToDecimal(participant.FundraisingGoal);

                            command.Parameters.Add("@CreatedDateUTC", System.Data.SqlDbType.DateTime);
                            command.Parameters["@CreatedDateUTC"].Value = participant.CreatedDateUTC.DateTime;

                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    SqlConnection.Close();
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                // Handle the error
            }

            return isSuccess;
        }

        public static async Task<bool> WriteDonationsToSqlAsync(IList<DonationTableEntity> donations, string sqlConnectionString)
        {
            bool isSuccess = false;
            
            using (var SqlConnection = new SqlConnection(sqlConnectionString))
            {
                await SqlConnection.OpenAsync();

                foreach (var donation in donations)
                {
                    try
                    {
                        using (var command = new SqlCommand("InsertDonation", SqlConnection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.CommandTimeout = 90;

                            command.Parameters.Add("@DonationId", System.Data.SqlDbType.UniqueIdentifier);
                            command.Parameters["@DonationId"].Value = new Guid(donation.RowKey);

                            command.Parameters.Add("@ExtraLifeDonationId", System.Data.SqlDbType.NVarChar);
                            command.Parameters["@ExtraLifeDonationId"].Value = donation.DonorId;

                            command.Parameters.Add("@ParticipantId", System.Data.SqlDbType.UniqueIdentifier);
                            command.Parameters["@ParticipantId"].Value = new Guid(donation.ParticipantUniqueIdentifier);

                            command.Parameters.Add("@ExtraLifeParticipantId", System.Data.SqlDbType.BigInt);
                            command.Parameters["@ExtraLifeParticipantId"].Value = donation.ParticipantId;

                            command.Parameters.Add("@DonorName", System.Data.SqlDbType.NVarChar);
                            command.Parameters["@DonorName"].Value = donation.DisplayName ?? string.Empty;

                            command.Parameters.Add("@TeamId", System.Data.SqlDbType.BigInt);
                            command.Parameters["@TeamId"].Value = donation.TeamId;

                            command.Parameters.Add("@Amount", System.Data.SqlDbType.Decimal);
                            command.Parameters["@Amount"].Value = Convert.ToDecimal(donation.Amount);

                            command.Parameters.Add("@Message", System.Data.SqlDbType.NVarChar);
                            command.Parameters["@Message"].Value = donation.Message ?? string.Empty;

                            command.Parameters.Add("@AvatarImageURL", System.Data.SqlDbType.NVarChar);
                            command.Parameters["@AvatarImageURL"].Value = donation.AvatarImageURL ?? string.Empty;

                            command.Parameters.Add("@CreatedDateUTC", System.Data.SqlDbType.DateTime);
                            command.Parameters["@CreatedDateUTC"].Value = donation.CreatedDateUTC.DateTime;

                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the error
                    }
                }
                SqlConnection.Close();
            }

            isSuccess = true;

            return isSuccess;
        }
    }
}
