using System.Net.Sockets;
using System.Net;
using System.Text;
using ApiInterface.InternalModels;
using System.Text.Json;
using ApiInterface.Exceptions;
using ApiInterface.Processors;
using ApiInterface.Models;

namespace ApiInterface
{
    public class Server
    {
        private static IPEndPoint serverEndPoint = new(IPAddress.Loopback, 11000);
        private static int supportedParallelConnections = 1;

        
        public static async Task Start()
        {
            using Socket listener = new(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(serverEndPoint);
            listener.Listen(supportedParallelConnections);
            Console.WriteLine($"Server ready at {serverEndPoint}");

            while (true)
            {
                var handler = await listener.AcceptAsync();
                try
                {
                    // Recibe el mensaje crudo (consulta SQL)
                    var rawMessage = GetMessage(handler);

                    // Verificar si el tipo de solicitud es válido
                    if (string.IsNullOrEmpty(rawMessage))
                    {
                        throw new InvalidRequestException("Request is empty or null.");
                    }

                    // Crear el objeto Request usando el constructor que recibe los parámetros
                    var request = new Request
                    {
                        RequestType = RequestType.SQLSentence,  // Definir el tipo de solicitud
                        RequestBody = rawMessage                // Asignar la consulta SQL
                    };

                    // Validar el tipo de solicitud
                    if (request.RequestType != RequestType.SQLSentence)
                    {
                        throw new UnknownRequestTypeException($"Error desconocido: {request.RequestType}");
                    }

                    // Procesar la consulta usando el Query Processor
                    var processor = new SQLSentenceProcessor(request);
                    var response = processor.Process();

                    // Enviar la respuesta al cliente usando SendResponse
                    SendResponse(response, handler);
                }
                catch (InvalidRequestException ex)
                {
                    Console.WriteLine($"Error por solicitud inválida: {ex.Message}");
                    // Manejo adicional o log
                }
                catch (UnknownRequestTypeException ex)
                {
                    Console.WriteLine($"Error desconocido: {ex.Message}");
                    // Manejo adicional o log
                }
                catch (Exception ex)
                {
                    // Manejo de errores generales
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }



        private static string GetMessage(Socket handler)
        {
            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadLine() ?? String.Empty;
            }
        }

        private static Request ConvertToRequestObject(string rawMessage)
        {
            return JsonSerializer.Deserialize<Request>(rawMessage) ?? throw new InvalidRequestException("Ocurrió un error en la ejecución");
        }

        private static Response ProcessRequest(Request requestObject)
        {
            var processor = ProcessorFactory.Create(requestObject);
            return processor.Process();
        }

        private static void SendResponse(Response response, Socket handler)
        {
            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(JsonSerializer.Serialize(response));
            }
        }

        private static Task SendErrorResponse(string reason, Socket handler)
        {
            throw new NotImplementedException();
        }

        
    }
}
