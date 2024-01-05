global using Http.Net.Hosting;
global using Http.Net.Loger;
global using System;
global using System.IO;
global using System.IO.Compression;
global using System.Linq;
global using System.Net;
global using System.Net.Sockets;
global using System.Text;
global using System.Threading.Tasks;
global using System.Collections.Generic;

// host start
var app = new WebApplication();
await app.RunAsycn();
