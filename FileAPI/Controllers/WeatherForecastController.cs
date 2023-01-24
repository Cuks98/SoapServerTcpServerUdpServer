using ConsoleApp1;
using DataBaseAPI.Data;
using DataBaseAPI.Models;
using GymMenagmentApp.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SautinSoft.Document;
using SautinSoft.Document.Tables;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Wkhtmltopdf.NetCore;

namespace FileAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        readonly IGeneratePdf _generatePdf;
        private readonly DataContext _context;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IGeneratePdf generatePdf, DataContext context)
        {
            _logger = logger;
            _generatePdf = generatePdf;
            _context = context;
        }





        [HttpGet("get-rtf")]
        public async Task<string> GetRtfApi()
        {
            var filepath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\SimpleTable.rtf";

            using (var fileInput = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                MemoryStream memoryStream = new MemoryStream();
                await fileInput.CopyToAsync(memoryStream);

                var buffer = memoryStream.ToArray();
                return Convert.ToBase64String(buffer);
            }
        }

        [HttpPost("create-rtf")]
        public bool AddSimpleTable(List<ReportData> reportData)
        {
            string documentPath = @"SimpleTable.rtf";


            DocumentCore dc = new DocumentCore();


            Section s = new Section(dc);
            dc.Sections.Add(s);


            Table table = new Table(dc);
            double width = LengthUnitConverter.Convert(180, LengthUnit.Millimeter, LengthUnit.Point);
            table.TableFormat.PreferredWidth = new TableWidth(width, TableWidthUnit.Point);
            table.TableFormat.Alignment = HorizontalAlignment.Center;

            int counter = 0;


            int rows = reportData.Count + 1;
            int columns = 6;
            for (int r = 0; r < rows; r++)
            {
                TableRow row = new TableRow(dc);


                for (int c = 0; c < columns; c++)
                {
                    TableCell cell = new TableCell(dc);


                    cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, BorderStyle.Dotted, Color.Black, 1.0);


                    cell.CellFormat.PreferredWidth = new TableWidth(width / columns, TableWidthUnit.Point);


                    if (r == 0)
                    {
                        cell.CellFormat.BackgroundColor = new Color("#358CCB");
                    }

                    row.Cells.Add(cell);


                    Paragraph p = new Paragraph(dc);
                    p.ParagraphFormat.Alignment = HorizontalAlignment.Center;
                    p.ParagraphFormat.SpaceBefore = LengthUnitConverter.Convert(3, LengthUnit.Millimeter, LengthUnit.Point);
                    p.ParagraphFormat.SpaceAfter = LengthUnitConverter.Convert(3, LengthUnit.Millimeter, LengthUnit.Point);

                    //p.Content.Start.Insert(String.Format("{0}", (char)(counter + 'A')), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#3399FF"), Size = 12.0 });
                    if (r == 0)
                    {
                        if (c == 0)
                        {
                            p.Content.Start.Insert(String.Format("ID"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                        else if (c == 1)
                        {
                            p.Content.Start.Insert(String.Format("MemberID"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                        else if (c == 2)
                        {
                            p.Content.Start.Insert(String.Format("Name"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                        else if (c == 3)
                        {
                            p.Content.Start.Insert(String.Format("Last Name"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                        else if (c == 4)
                        {
                            p.Content.Start.Insert(String.Format("Date"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                        else if (c == 5)
                        {
                            p.Content.Start.Insert(String.Format("Amount"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                    }
                    else
                    {
                        //p.Content.Start.Insert(String.Format("{0}", (char)(counter + 'A')), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        if (c == 0)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].Id}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                        else if (c == 1)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].UserId}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                        else if (c == 2)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].Name}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                        else if (c == 3)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].LastName}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                        else if (c == 4)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].Date}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                        else if (c == 5)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].Amount}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                    }
                    cell.Blocks.Add(p);
                    counter++;
                }
                table.Rows.Add(row);
            }


            s.Blocks.Add(table);


            dc.Save(documentPath, new RtfSaveOptions() { EmbeddedJpegQuality = 95 });
            return true;

        }

        [HttpPost("create-pdf")]
        public bool AddSimpleTablePdf(List<ReportData> reportData)
        {
            string documentPath = @"SimpleTable.pdf";


            DocumentCore dc = new DocumentCore();


            Section s = new Section(dc);
            dc.Sections.Add(s);


            Table table = new Table(dc);
            double width = LengthUnitConverter.Convert(180, LengthUnit.Millimeter, LengthUnit.Point);
            table.TableFormat.PreferredWidth = new TableWidth(width, TableWidthUnit.Point);
            table.TableFormat.Alignment = HorizontalAlignment.Center;

            int counter = 0;


            int rows = reportData.Count + 1;
            int columns = 6;
            for (int r = 0; r < rows; r++)
            {
                TableRow row = new TableRow(dc);


                for (int c = 0; c < columns; c++)
                {
                    TableCell cell = new TableCell(dc);


                    cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, BorderStyle.Dotted, Color.Black, 1.0);


                    cell.CellFormat.PreferredWidth = new TableWidth(width / columns, TableWidthUnit.Point);


                    if (r == 0)
                    {
                        cell.CellFormat.BackgroundColor = new Color("#358CCB");
                    }

                    row.Cells.Add(cell);


                    Paragraph p = new Paragraph(dc);
                    p.ParagraphFormat.Alignment = HorizontalAlignment.Center;
                    p.ParagraphFormat.SpaceBefore = LengthUnitConverter.Convert(3, LengthUnit.Millimeter, LengthUnit.Point);
                    p.ParagraphFormat.SpaceAfter = LengthUnitConverter.Convert(3, LengthUnit.Millimeter, LengthUnit.Point);

                    //p.Content.Start.Insert(String.Format("{0}", (char)(counter + 'A')), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#3399FF"), Size = 12.0 });
                    if (r == 0)
                    {
                        if (c == 0)
                        {
                            p.Content.Start.Insert(String.Format("ID"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                        else if (c == 1)
                        {
                            p.Content.Start.Insert(String.Format("MemberID"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                        else if (c == 2)
                        {
                            p.Content.Start.Insert(String.Format("Name"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                        else if (c == 3)
                        {
                            p.Content.Start.Insert(String.Format("Last Name"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                        else if (c == 4)
                        {
                            p.Content.Start.Insert(String.Format("Date"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                        else if (c == 5)
                        {
                            p.Content.Start.Insert(String.Format("Amount"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#FFFFFF"), Size = 12.0 });
                        }
                    }
                    else
                    {
                        //p.Content.Start.Insert(String.Format("{0}", (char)(counter + 'A')), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        if (c == 0)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].Id}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                        else if (c == 1)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].UserId}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                        else if (c == 2)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].Name}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                        else if (c == 3)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].LastName}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                        else if (c == 4)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].Date}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                        else if (c == 5)
                        {
                            p.Content.Start.Insert(String.Format($"{reportData[r - 1].Amount}"), new CharacterFormat() { FontName = "Arial", FontColor = new Color("#000000"), Size = 12.0 });
                        }
                    }
                    cell.Blocks.Add(p);
                    counter++;
                }
                table.Rows.Add(row);
            }


            s.Blocks.Add(table);


            dc.Save(documentPath, new PdfSaveOptions() { Compliance = PdfCompliance.PDF_A1a });

            return true;

        }


        [HttpGet("get-pdf")]
        public async Task<string> GetPdfApi()
        {
            var filepath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\SimpleTable.pdf";

            using (var fileInput = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                MemoryStream memoryStream = new MemoryStream();
                await fileInput.CopyToAsync(memoryStream);

                var buffer = memoryStream.ToArray();
                return Convert.ToBase64String(buffer);
            }
        }






        [HttpPost("add-record-to-xml")]
        public async Task<bool> AddRecordToXmlFile(Employee employee)
        {
            try
            {
                AddRecordsToXml(employee);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet("get-login-data")]
        public async Task<List<LoginData>> GetLoginHistory()
        {
            var res = new List<LoginData>();
            var filePath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\FileData\XMLFile.xml";

            XDocument xdoc = XDocument.Load(filePath);

            var xroot = xdoc.Root;

            var childNodes = xdoc.Root.Descendants("LOGINDATA");

            foreach (var item in childNodes)
            {
                LoginData loginData = new LoginData()
                {
                    Id = int.Parse(item.Attribute("id").Value),
                    EmployeeId = int.Parse(item.Descendants("EMPLOYEEID").FirstOrDefault().Value),
                    DateTime = DateTime.Parse(item.Descendants("DATETIME").FirstOrDefault().Value)
                };
                res.Add(loginData);
            }
            return res;
        }

        [HttpGet("get-single-login-data")]
        public async Task<LoginData> GetSingleLoginHistory(int id)
        {
            var res = new List<LoginData>();
            var filePath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\FileData\XMLFile.xml";

            XDocument xdoc = XDocument.Load(filePath);

            var xroot = xdoc.Root;

            var childNodes = xdoc.Root.Descendants("LOGINDATA");

            foreach (var item in childNodes)
            {
                if (item.Attribute("id")?.Value == id.ToString())
                {
                    LoginData loginData = new LoginData()
                    {
                        Id = int.Parse(item.Attribute("id").Value),
                        EmployeeId = int.Parse(item.Descendants("EMPLOYEEID").FirstOrDefault().Value),
                        DateTime = DateTime.Parse(item.Descendants("DATETIME").FirstOrDefault().Value)
                    };
                    return loginData;
                }
            }
            return null;
        }

        [HttpPost("update-login-data")]
        public async Task<bool> UpdateLoginHistory(LoginData data)
        {
            data.DateTime = DateTime.Now;
            var res = new List<LoginData>();
            var filePath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\FileData\XMLFile.xml";

            XDocument xdoc = XDocument.Load(filePath);

            var xroot = xdoc.Root;

            var childNodes = xdoc.Root.Descendants("LOGINDATA");

            foreach (var item in childNodes)
            {
                if (int.Parse(item.Attribute("id").Value) == data.Id)
                {
                    item.Descendants("EMPLOYEEID").FirstOrDefault().Value = data.EmployeeId.ToString();
                    item.Descendants("DATETIME").FirstOrDefault().Value = data.DateTime.ToString();
                }
            }
            xdoc.Save(filePath);
            return true;
        }

        [HttpDelete("delete-login-data")]
        //[Authorize(Roles = "1,2")]
        public bool DeleteLoginHistory()
        {
            var filePath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\FileData\XMLFile.xml";

            var doc = new XmlDocument();
            doc.Load(filePath);

            var root = doc.SelectSingleNode("LOGINDATAS");
            root.RemoveAll();

            doc.Save(filePath);


            return true;
        }






        [HttpPost("add-transacton-data-to-json")]
        public bool AddTransactionData(ExtendMembershipRequest extendMembershipRequest)
        {
            try
            {
                var filePath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\FileData\TransactionData.json";

                var jsonData = System.IO.File.ReadAllText(filePath);
                var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(jsonData) ?? new List<Transaction>();

                var id = transactionList.Last().Id + 1;
                var transaction = new Transaction() { Id = id, UserId = extendMembershipRequest.Id, Date = DateTime.Now, Amount = (extendMembershipRequest.NumberOfMonths * 240).ToString() };

                transactionList.Add(transaction);

                jsonData = JsonConvert.SerializeObject(transactionList);

                System.IO.File.WriteAllText(filePath, jsonData);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        [HttpGet("get-transaction-data")]
        public async Task<List<Transaction>> GetTransactionData()
        {
            var filePath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\FileData\TransactionData.json";
            var jsonData = System.IO.File.ReadAllText(filePath);
            var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(jsonData) ?? new List<Transaction>();

            return transactionList;
        }

        [HttpGet("get-single-transaction-data")]
        public async Task<Transaction> GetSingleTransactionData(int id)
        {
            var filePath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\FileData\TransactionData.json";
            var jsonData = System.IO.File.ReadAllText(filePath);
            var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(jsonData) ?? new List<Transaction>();

            var transaction = transactionList.Where(x => x.Id == id).FirstOrDefault();

            if (transaction != null)
            {
                return transaction;

            }

            return null;
        }

        [HttpDelete("remove-transaction-data")]
        public async Task<bool> RemoveTransactionData(int id)
        {
            var filePath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\FileData\TransactionData.json";
            var jsonData = System.IO.File.ReadAllText(filePath);
            var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(jsonData) ?? new List<Transaction>();

            var transactionToRemove = transactionList.Where(x => x.Id == id).FirstOrDefault();
            if (transactionToRemove != null)
            {
                transactionList.Remove(transactionToRemove);

                jsonData = JsonConvert.SerializeObject(transactionList);

                System.IO.File.WriteAllText(filePath, jsonData);

                return true;
            }
            return false;
        }

        [HttpPost("update-transaction-data")]
        public bool UpdateTransactionData(Transaction request)
        {
            var filePath = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\FileData\TransactionData.json";
            var jsonData = System.IO.File.ReadAllText(filePath);
            var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(jsonData) ?? new List<Transaction>();


            foreach (var transaction in transactionList)
            {
                if (transaction.Id == request.Id)
                {
                    transaction.UserId = request.UserId;
                    transaction.Date = request.Date;
                    transaction.Amount = request.Amount;
                }
            }

            jsonData = JsonConvert.SerializeObject(transactionList);

            System.IO.File.WriteAllText(filePath, jsonData);

            return true;
        }





        [HttpPost("claculate-tcp")]
        public UdpResponce Calculate(TcpServerRequest tcpServerRequest)
        {
            TcpClient tcpClient = new TcpClient("127.0.0.1", 13000);
            NetworkStream stream = tcpClient.GetStream();
            var data = "";
            try
            {
                string poruka = System.Text.Json.JsonSerializer.Serialize(tcpServerRequest);


                byte[] sendData = Encoding.UTF8.GetBytes(poruka);

                stream.Write(sendData, 0, sendData.Length);

                byte[] bytes = new byte[512];
                int size;
                data = "";
                size = stream.Read(bytes, 0, bytes.Length);
                data = Encoding.UTF8.GetString(bytes, 0, size);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //Console.WriteLine("Closing tcp client");
                stream.Close();
                stream.Dispose();
                tcpClient.Close();
                tcpClient.Dispose();
                //Console.WriteLine("Closed tcp client");
            }
            var udpResponce = new UdpResponce()
            {
                responce = data
            };
            stream.Close();
            stream.Dispose();
            tcpClient.Close();
            tcpClient.Dispose();

            return udpResponce;
        }




        [HttpPost("claculate-udp")]
        public UdpResponce CalculateUdp(TcpServer.UdpServerRequest udpServerRequest)
        {
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
            client.Connect(ep);

            string poruka = System.Text.Json.JsonSerializer.Serialize(udpServerRequest);


            byte[] sendData = Encoding.UTF8.GetBytes(poruka);

            client.Send(sendData);

            // then receive data
            var receivedData = client.Receive(ref ep);

            byte[] bytes = new byte[512];
            var data = "";
            data = Encoding.UTF8.GetString(receivedData, 0, receivedData.Length);

            var udpResponce = new UdpResponce()
            {
                responce = data
            };

            return udpResponce;
        }






        [HttpPost("post-mail")]
        public async Task<ActionResult> sendEmail(MailRequest request)
        {
            List<Task<ActionResult>> actionResults = new List<Task<ActionResult>>();
            //var listOfWorkinkAddresses = new List<string>() { "filip.curin69@gmial.com", "filip.curin@gdi.net", "fcurin@tvz.hr"};
            //foreach (var address in listOfWorkinkAddresses)
            //{
            //    actionResults.Add(SendMailToMailAddress(request.Subject, request.Body, address));
            //}
            foreach (var address in request.ListOfMailAddresses)
            {
                actionResults.Add(SendMailToMailAddress(request.Subject, request.Body, address));
            }

            var result = await Task.WhenAll(actionResults);

            return new OkResult();
        }
        [HttpPost("post-single-mail")]
        public async Task<ActionResult> SendMailToMailAddress(string subject, string body, string adderess)
        {
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            var smtpUsername = "gym.app365";
            var smtpPassword = "tpjo gjcq mkex hufh";
            var smtpFrom = "gym.app365@gmail.com";



            //var smtpTo = "filip.curin69@gmail.com";
            var emailSubject = subject;
            var emailBody = body;


            using (var stmpClient = new SmtpClient()
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                Timeout = 20000
            })
            {
                var mail = new MailMessage()
                {
                    Body = emailBody,
                    Subject = emailSubject,
                    From = new MailAddress(smtpFrom, "GymApp")
                };
                mail.To.Add(adderess);
                try
                {
                    await stmpClient.SendMailAsync(mail);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return new OkResult();
        }



        [HttpGet("get-trainers")]
        public async Task<TrainerResponse> GetTrainers()
        {
            var soapTrainer = new TrainerSoapService.SoapServiceClient();
            var trainers = await soapTrainer.GetTrainersAsync();
            return new TrainerResponse
            {
                Trainers = trainers.Body.GetTrainersResult.Trainers.Select(x => new Trainer
                {
                    DoB = x.DoB,
                    FirstName = x.FirstName,
                    Gender = x.Gender,
                    Height = x.Height,
                    Id = x.Id,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Speciality = x.Speciality,
                    SportHistory = x.SportHistory,
                    Weight = x.Weight
                }).ToList(),
                ErrorId = trainers.Body.GetTrainersResult.ErrorId,
                ErrorMsg = trainers.Body.GetTrainersResult.ErrorMsg
            };
        }

        [HttpGet("get-trainer")]
        public async Task<TrainerResponse> GetTrainer(int id)
        {
            var soapTrainer = new TrainerSoapService.SoapServiceClient();
            var trainers = await soapTrainer.GetTrainerAsync(id);
            return new TrainerResponse
            {
                Trainers = trainers.Body.GetTrainerResult.Trainers.Select(x => new Trainer
                {
                    DoB = x.DoB,
                    FirstName = x.FirstName,
                    Gender = x.Gender,
                    Height = x.Height,
                    Id = x.Id,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Speciality = x.Speciality,
                    SportHistory = x.SportHistory,
                    Weight = x.Weight
                }).ToList(),
                ErrorId = trainers.Body.GetTrainerResult.ErrorId,
                ErrorMsg = trainers.Body.GetTrainerResult.ErrorMsg
            };
        }

        [HttpPost("register-new-trainer")]
        public async Task<TrainerResponse> RegisterNewTrainer(ConsoleApp1.Trainer trainer)
        {
            var soapTrainer = new TrainerSoapService.SoapServiceClient();
            var trainers = await soapTrainer.RegisterNewTrainerAsync(new TrainerSoapService.Trainer
            {
                DoB = trainer.DoB,
                FirstName = trainer.FirstName,
                Gender = trainer.Gender,
                Height = trainer.Height,
                Id = trainer.Id,
                LastName = trainer.LastName,
                PhoneNumber = trainer.PhoneNumber,
                Speciality = trainer.Speciality,
                SportHistory = trainer.SportHistory,
                Weight = trainer.Weight
            });
            return new TrainerResponse
            {
                Trainers = trainers.Body.RegisterNewTrainerResult.Trainers.Select(x => new Trainer
                {
                    DoB = x.DoB,
                    FirstName = x.FirstName,
                    Gender = x.Gender,
                    Height = x.Height,
                    Id = x.Id,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Speciality = x.Speciality,
                    SportHistory = x.SportHistory,
                    Weight = x.Weight
                }).ToList(),
                ErrorId = trainers.Body.RegisterNewTrainerResult.ErrorId,
                ErrorMsg = trainers.Body.RegisterNewTrainerResult.ErrorMsg
            };
        }

        [HttpPut("update-trainer")]
        public async Task<TrainerResponse> UpdateTrainer(Trainer trainer)
        {
            var soapTrainer = new TrainerSoapService.SoapServiceClient();
            var trainers = await soapTrainer.UpdateTrainerAsync(new TrainerSoapService.Trainer
            {
                DoB = trainer.DoB,
                FirstName = trainer.FirstName,
                Gender = trainer.Gender,
                Height = trainer.Height,
                Id = trainer.Id,
                LastName = trainer.LastName,
                PhoneNumber = trainer.PhoneNumber,
                Speciality = trainer.Speciality,
                SportHistory = trainer.SportHistory,
                Weight = trainer.Weight
            });
            return new TrainerResponse
            {
                Trainers = trainers.Body.UpdateTrainerResult.Trainers.Select(x => new Trainer
                {
                    DoB = x.DoB,
                    FirstName = x.FirstName,
                    Gender = x.Gender,
                    Height = x.Height,
                    Id = x.Id,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Speciality = x.Speciality,
                    SportHistory = x.SportHistory,
                    Weight = x.Weight
                }).ToList(),
                ErrorId = trainers.Body.UpdateTrainerResult.ErrorId,
                ErrorMsg = trainers.Body.UpdateTrainerResult.ErrorMsg
            };
        }

        [HttpDelete("remove-trainer")]
        public async Task<TrainerResponse> RemoveTrainer(int id)
        {
            var soapTrainer = new TrainerSoapService.SoapServiceClient();
            var trainers = await soapTrainer.DeleteTrainerAsync(id);
            return new TrainerResponse
            {
                Trainers = trainers.Body.DeleteTrainerResult.Trainers.Select(x => new Trainer
                {
                    DoB = x.DoB,
                    FirstName = x.FirstName,
                    Gender = x.Gender,
                    Height = x.Height,
                    Id = x.Id,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Speciality = x.Speciality,
                    SportHistory = x.SportHistory,
                    Weight = x.Weight
                }).ToList(),
                ErrorId = trainers.Body.DeleteTrainerResult.ErrorId,
                ErrorMsg = trainers.Body.DeleteTrainerResult.ErrorMsg
            };
        }



        private static void AddRecordsToXml(Employee employee)
        {
            string xmlFileUrl = @"C:\Users\filip.curin\source\repos\FileAPI\FileAPI\FileData\XMLFile.xml";

            XmlDocument doc = new XmlDocument();

            //doc.Load(@"C:\Users\filip.curin\Desktop\XMLFile.xml");
            doc.Load(xmlFileUrl);
            XmlNode root = doc.SelectSingleNode("LOGINDATAS");

            XmlElement loginData = doc.CreateElement("LOGINDATA");

            root.AppendChild(loginData);

            XmlAttribute id = doc.CreateAttribute("id");
            id.Value = doc.SelectNodes("LOGINDATAS/LOGINDATA").Count.ToString();
            loginData.Attributes.Append(id);

            XmlElement EmployeeId = doc.CreateElement("EMPLOYEEID");
            EmployeeId.InnerText = employee.Id.ToString();
            loginData.AppendChild(EmployeeId);
            XmlElement DateTimeString = doc.CreateElement("DATETIME");
            DateTimeString.InnerText = DateTime.Now.ToString();
            loginData.AppendChild(DateTimeString);
            
            
            doc.Save(xmlFileUrl);

            //Console.WriteLine(doc.InnerXml);
        }
    }
}