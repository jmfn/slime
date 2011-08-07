using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using AwesomiumSharp;
using NDesk.Options;

namespace Slime {
    class Program {
        private static bool finishedLoading = false;

        public static void Main(string[] args) {
            string url;
            bool getSource = false;
            bool getRender = false;
            bool getFullRender = false;
            bool getPageDims = false;
            bool getNetworkResources = false;
            bool runJavascript = false;
            string javascript = "";
            bool help = false;
            int delay = 0;
            var dim = new Point(1024, 768);

            var p = new OptionSet() {
                                        {"source", v => { getSource = true; }},
                                        {
                                            "render", v => {
                                                          getRender = true;
                                                          getFullRender = false;
                                                      }
                                            },
                                        {
                                            "fullrender", v => {
                                                              getRender = true;
                                                              getFullRender = true;
                                                          }
                                            },
                                        {"pagedims", v => { getPageDims = true; }},
                                        {
                                            "dims=", v => {
                                                        var d = v.Split('x');
                                                        if (d.Length != 2) return;
                                                        dim.X = Convert.ToInt32(d[0]);
                                                        dim.Y = Convert.ToInt32(d[1]);
                                                    }
                                            },
                                        {
                                            "js=", v => {
                                                      runJavascript = true;
                                                      javascript = v;
                                                  }
                                            },
                                        {"network", v => { getNetworkResources = true; }},
                                        {"delay=", v => { delay = Convert.ToInt32(v.Trim()); }},
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
            
            var slimer = new Slimer(url, dim.X, dim.Y, delay);

            if (getSource) {
                Console.WriteLine(slimer.GetHtml());
            }

            if (getPageDims) {
                var d = slimer.GetPageDimensions();
                Console.WriteLine(d.X + " x " + d.Y);
            }

            if (runJavascript) {
                var response = slimer.JavascriptRunner(javascript);
                Console.WriteLine("Javascript response:");
                Console.WriteLine(response);
            }

            if (getRender) {
                var render = slimer.GetRender(getFullRender);
                render.Save("result.jpg", ImageFormat.Jpeg);
                Console.WriteLine("Render complete...Loading it for you.");
                Process.Start("result.jpg");
            }

            if (getNetworkResources) {
                var resources = slimer.GetNetworkResources();
                foreach (var item in resources) {
                    Console.WriteLine(item);
                }
            }

            slimer.Dispose();
        }

        static void ShowHelp(OptionSet p) {
            Console.WriteLine("Usage: slime [options]+ url");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
