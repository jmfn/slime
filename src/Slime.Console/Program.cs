using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using AwesomiumSharp;
using NDesk.Options;

namespace Slime {
    class Program {
        private static bool finishedLoading = false;

        static void Main(string[] args) {
            string url;
            bool getSource = false;
            bool getRender = false;
            bool getFullRender = false;
            bool getPageDims = false;
            bool runJavascript = false;
            string javascript = "";
            bool help = false;
            int verbose = 0;
            var p = new OptionSet() {
                                        {"source", v => { getSource = true; }},
                                        {
                                            "render", v => {
                                                          getRender = true;
                                                          getFullRender = false;
                                                      }
                                            },
                                        {"fullrender", v => {
                                                           getRender = true;
                                                           getFullRender = true;
                                                       }
                                            },
                                        {"dims", v => { getPageDims = true; }},
                                        {
                                            "js", v => {
                                                      runJavascript = true;
                                                      javascript = v;
                                                  }
                                            },
                                        {"v|verbose", v => { ++verbose; }},
                                        {"h|?|help", v => help = v != null},
                                    };
            var extra = p.Parse(args);

            if (extra.Count > 0) {
                url = string.Join(" ", extra.ToArray());
            }
            else {
                ShowHelp(p);
                return;
            }

            var slimer = new Slimer(url, 1024, 768);

            if (getSource) {
                Console.WriteLine(slimer.GetHtml());
            }

            if (getPageDims) {
                var dim = slimer.GetPageDimensions();
                Console.WriteLine(dim.X + " x " + dim.Y);
            }

            if (runJavascript) {
                var response = slimer.JavascriptRunner(javascript);
                Console.WriteLine("Javascript response:");
                Console.WriteLine(response);
            }

            if (getRender) {
                slimer.GetRender(getFullRender).SaveToPNG("result.png", true);
                Console.WriteLine("Render complete...Loading it for you.");
                Process.Start("result.png");
            }

            slimer.Dispose();
        }

        static void ShowHelp(OptionSet p) {
            Console.WriteLine("Usage: slime [options]+ url");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        public static void Sandbox(string url) {
            // Some informative message.
            Console.WriteLine("Getting a 1024x768 snapshot of " + url + " ...");

            using (WebView webView = WebCore.CreateWebView(1024, 768, true)) {
                webView.LoadURL(url);
                webView.LoadCompleted += OnFinishLoading;

                while (!finishedLoading) {
                    Thread.Sleep(50);
                    // A Console application does not have a synchronization
                    // context, thus auto-update won't be enabled on WebCore.
                    // We need to manually call Update here.
                    WebCore.Update();
                }

                // Render to a pixel buffer and save the buffer
                // to a .png image.
                webView.Render().SaveToPNG("result.png", true);
            }

            // Announce.
            Console.Write("Hit any key to see the result...");
            Console.ReadKey(true);

            // Start the application associated with .png files
            // and display the file.
            Process.Start("result.png");

            // Shut down Awesomium before exiting.
            WebCore.Shutdown();
        }

        private static void OnFinishLoading(object sender, EventArgs e) {
            finishedLoading = true;
        }
    }
}
