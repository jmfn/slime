using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using AwesomiumSharp;

namespace Slime {
    public class Slimer : IDisposable {
        public WebView Browser { get; private set; }

        public IList<string> Requests { get; private set; }
        public string Url { get; private set; }
        public Point Dimensions { get; private set; }
        public int MillisecondsDelayLoad { get; private set; }
        public bool IsFinishedLoading { get; private set; }

        public Slimer(string url, int width, int height) {
            init(url, width, height, 0);
        }

        public Slimer(string url, int width, int height, int millisecondsDelayLoad) {
            init(url, width, height, millisecondsDelayLoad);
        }

        private void init(string url, int width, int height, int millisecondsDelayLoad) {
            Url = url;
            Dimensions = new Point(width, height);
            MillisecondsDelayLoad = millisecondsDelayLoad;
            loadPage();
        }

        private void loadPage() {
            IsFinishedLoading = false;
            Requests = new List<string>();

            // Cap dimensions @ 2000 pixels
            var x = Dimensions.X > 2000 ? 2000 : Dimensions.X;
            var y = Dimensions.Y > 2000 ? 2000 : Dimensions.Y;
            Dimensions = new Point(x, y);

            Browser = WebCore.CreateWebView(Dimensions.X, Dimensions.Y, false);
            Browser.LoadCompleted += OnFinishLoading;
            Browser.ResourceResponse += OnResourceResponse;
            Browser.LoadURL(Url);
            WaitForLoad(MillisecondsDelayLoad);
        }

        private void OnFinishLoading(object sender, EventArgs e) {
            IsFinishedLoading = true;
        }

        public RenderBuffer GetRender(bool useFullPageHeight = false) {
            checkForLoading();

            if (useFullPageHeight) {
                Dimensions = GetPageDimensions();
                Dispose();
                loadPage(); // reload page with actual page dimensions
            }

            return Browser.Render();
        }

        public string GetHtml() {
            checkForLoading();

            return Browser.ExecuteJavascriptWithResult("document.documentElement.innerHTML;").ToString();
        }

        public Point GetPageDimensions() {
            checkForLoading();

            var width = Browser.ExecuteJavascriptWithResult("document.body.clientWidth;");
            var height = Browser.ExecuteJavascriptWithResult("document.body.clientHeight;");

            return new Point(width.ToInteger(), height.ToInteger());
        }

        public string JavascriptRunner(string javascript, int timeout = 0) {
            return Browser.ExecuteJavascriptWithResult(javascript, "", timeout).ToString();
        }

        private void checkForLoading() {
            if (!IsFinishedLoading) {
                throw new ApplicationException("Page not done loading. Poll for IsFinishedLoading.");
            }
        }

        public void Dispose() {
            WebCore.Shutdown();
        }

        public void WaitForLoad(int millisecondsDelayLoad = 0) {
            Trace.Write("Waiting for load to finish.");

            while (!IsFinishedLoading) {
                Thread.Sleep(50);
                WebCore.Update();
                Trace.Write(".");
            }

            if (millisecondsDelayLoad > 0) {
                Trace.WriteLine("Delaying load for: " + millisecondsDelayLoad + "ms.");
                Thread.Sleep(millisecondsDelayLoad);
            }

            Trace.WriteLine("done");
        }

        public IEnumerable<string> GetNetworkResources() {
            return Requests;
        }

        private void OnResourceResponse(object sender, ResourceResponseEventArgs e) {
            Requests.Add(e.Url);
            Trace.WriteLine("Response received from: " + e.Url);
        }
    }
}
