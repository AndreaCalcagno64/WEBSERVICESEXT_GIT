using System.Web.Http;
using System.Linq;
using System.Threading.Tasks;


namespace WEBSERVICESF2127.Controllers
{

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Helpers;
    using System.Web.Http;
    using Oracle.DataAccess.Client;

    [RoutePrefix("api/PR00ITERPROGETTOLettura")]
    public class PR00ITERPROGETTOLetturaController : ApiController
    {
        // --- DEFINIZIONE DEI DTO (Spostali qui o in un file Models.cs) ---
        public class F21_PR00_ITER_PROGETTORequest
        {
            public string CODPROGETTO { get; set; }
        }

        public class F21_PR00_ITER_PROGETTOResponse
        {
            public string CHIAVE { get; set; }
            public string CODPROGETTO { get; set; }
            public string COD_FASE { get; set; }
            public string DATA_INIZIO_PREVISTA { get; set; }
            public string DATA_INIZIO_PREVISTADATE { get; set; }
            public string DATA_FINE_PREVISTA { get; set; }
            public string DATA_FINE_PREVISTADATE { get; set; }
            public string DATA_INIZIO_EFFETTIVA { get; set; }
            public string DATA_INIZIO_EFFETTIVADATE { get; set; }
            public string DATA_FINE_EFFETTIVA { get; set; }
            public string DATA_FINE_EFFETTIVADATE { get; set; }
            public string ID { get; set; }
            public string DESCCOD_FASE { get; set; }
        }
        protected OracleConnection objConnectionORA;
        protected string strConnectionORA;
        protected string JSON = "";

        // [Authorize] // Protezione tramite Token
        [HttpPost]
        [Route("Lettura")]
        public async Task<IHttpActionResult> GetDettagli([FromBody] F21_PR00_ITER_PROGETTORequest request)
        {
            // ... controlli iniziali invariati ...

            try
            {
                string strConnectionORA = ConfigurationManager.ConnectionStrings["connectionStringORACLE"].ConnectionString;

                using (OracleConnection objConnectionORA = new OracleConnection(strConnectionORA.Trim()))
                {
                    string sql = @" SELECT * FROM (" +
                                        " SELECT UPPER(LTRIM(RTRIM(A.CODPROGETTO))||LTRIM(RTRIM(A.COD_FASE))) AS CHIAVE," +
                                        " A.CODPROGETTO AS CODPROGETTO," +
                                        " A.COD_FASE AS COD_FASE," +
                                        " TO_CHAR(A.DATA_INIZIO_PREVISTA,'DD/MM/YYYY') AS DATA_INIZIO_PREVISTA," +
                                        " A.DATA_INIZIO_PREVISTA AS DATA_INIZIO_PREVISTADATE," +
                                        " TO_CHAR(A.DATA_FINE_PREVISTA,'DD/MM/YYYY') AS DATA_FINE_PREVISTA," +
                                        " A.DATA_FINE_PREVISTA AS DATA_FINE_PREVISTADATE," +
                                        " TO_CHAR(A.DATA_INIZIO_EFFETTIVA,'DD/MM/YYYY') AS DATA_INIZIO_EFFETTIVA," +
                                        " A.DATA_INIZIO_EFFETTIVA AS DATA_INIZIO_EFFETTIVADATE," +
                                        " TO_CHAR(A.DATA_FINE_EFFETTIVA,'DD/MM/YYYY') AS DATA_FINE_EFFETTIVA," +
                                        " A.DATA_FINE_EFFETTIVA AS DATA_FINE_EFFETTIVADATE," +
                                        " A.ID," +
                                        " B.COD_FASE||'-'||B.DESCRIZIONE_FASE AS DESCCOD_FASE" +
                      " FROM F21_PR00_ITER_PROGETTO A, F21_TC41_FASI_PROCED B" +
                      " WHERE CODPROGETTO = :Codice" +
                     " and A.COD_FASE = B.COD_FASE" +
                     ")";

                    using (OracleCommand cmd = new OracleCommand(sql, objConnectionORA))
                    {
                        cmd.Parameters.Add("Codice", OracleDbType.Varchar2).Value = request.CODPROGETTO;
                        await objConnectionORA.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var risultato = new F21_PR00_ITER_PROGETTOResponse
                                {
                                    CHIAVE = reader["COD_FASE"] == DBNull.Value ? string.Empty : reader["CHIAVE"].ToString(),
                                    CODPROGETTO = reader["COD_FASE"] == DBNull.Value ? string.Empty : reader["CODPROGETTO"].ToString(),
                                    COD_FASE = reader["COD_FASE"] == DBNull.Value ? string.Empty : reader["COD_FASE"].ToString(),
                                    DATA_INIZIO_PREVISTA = reader["DATA_INIZIO_PREVISTA"] == DBNull.Value ? string.Empty : reader["DATA_INIZIO_PREVISTA"].ToString(),
                                    DATA_INIZIO_PREVISTADATE = reader["DATA_INIZIO_PREVISTADATE"] == DBNull.Value ? string.Empty : reader["DATA_INIZIO_PREVISTADATE"].ToString(),
                                    DATA_FINE_PREVISTA = reader["DATA_FINE_PREVISTA"] == DBNull.Value ? string.Empty : reader["DATA_FINE_PREVISTA"].ToString(),
                                    DATA_FINE_PREVISTADATE = reader["DATA_FINE_PREVISTADATE"] == DBNull.Value ? string.Empty : reader["DATA_FINE_PREVISTADATE"].ToString(),
                                    DATA_INIZIO_EFFETTIVA = reader["DATA_INIZIO_EFFETTIVA"] == DBNull.Value ? string.Empty : reader["DATA_INIZIO_EFFETTIVA"].ToString(),
                                    DATA_INIZIO_EFFETTIVADATE = reader["DATA_INIZIO_EFFETTIVADATE"] == DBNull.Value ? string.Empty : reader["DATA_INIZIO_EFFETTIVADATE"].ToString(),
                                    DATA_FINE_EFFETTIVA = reader["DATA_FINE_EFFETTIVA"] == DBNull.Value ? string.Empty : reader["DATA_FINE_EFFETTIVA"].ToString(),
                                    DATA_FINE_EFFETTIVADATE = reader["DATA_FINE_EFFETTIVADATE"] == DBNull.Value ? string.Empty : reader["DATA_FINE_EFFETTIVADATE"].ToString(),
                                    ID = reader["ID"] == DBNull.Value ? string.Empty : reader["ID"].ToString(),
                                    DESCCOD_FASE = reader["DESCCOD_FASE"] == DBNull.Value ? string.Empty : reader["DESCCOD_FASE"].ToString()
                                };

                                // --- SOLUZIONE QUI ---
                                // Invece di manipolare stringhe, crea una lista che contenga l'oggetto.
                                // Il framework la convertirà automaticamente in formato JSON array [ { ... } ]
                                var listaRisultato = new List<F21_PR00_ITER_PROGETTOResponse> { risultato };

                                // Restituisci la lista tramite il metodo Ok()
                                return Json(listaRisultato);
                            }
                        }
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                string Errore = ex.Message;
                return InternalServerError();
            }
        }
    }
}