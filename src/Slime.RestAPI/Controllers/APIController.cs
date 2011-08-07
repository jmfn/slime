using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Slime.RestAPI.Controllers {
    public class APIController : Controller {
        public void About() {
            Response.Write(@"
Slime: browser shots, network resources, source code.<br/>
- Enabled by Awesomium based on the Chromium Project.<br/>
- jim.frantzen@gmail.com<br/><br/>

- Endpoints:<br/>
    /api/about<br/>
    /api/render<br/>
    /api/kitchensink<br/><br/>

    params: url, w (width), h (height), d (delay)
");
        }

        public void KitchenSink(string url, int w = 1024, int h = 768, int d = 0) {
            var dim = new Point(w, h);
            var delay = d;

            url = HttpUtility.UrlDecode(url);

            using (var slimer = new Slimer(url, dim.X, dim.Y, delay)) {
                var pageDims = slimer.GetPageDimensions();

                Response.Write(
                    Json(new {
                                 url = url,
                                 pagedims = new {width = pageDims.X, height = pageDims.Y},
                                 source = slimer.GetHtml(),
                                 resources = slimer.GetNetworkResources()
                             }, JsonRequestBehavior.AllowGet)
                    );
            }
        }

        public JsonResult Render(string url, int w = 1024, int h = 768, int d = 0, bool base64 = false) {
            var dim = new Point(w, h);
            var delay = d;

            url = HttpUtility.UrlDecode(url);

            using (var slimer = new Slimer(url, dim.X, dim.Y, delay)) {
                slimer.RenderPath = HttpRuntime.CodegenDir;

                using (var render = slimer.GetRender()) {

                    if (!base64) {
                        Response.ContentType = "image/jpeg";
                        render.Save(Response.OutputStream, ImageFormat.Jpeg);
                        Response.End();
                        return null;
                    }
                    else {
                        string base64String;

                        using (var ms = new MemoryStream()) {
                            // Convert Image to byte[]
                            render.Save(ms, ImageFormat.Jpeg);
                            byte[] imageBytes = ms.ToArray();

                            // Convert byte[] to Base64 String
                            base64String = Convert.ToBase64String(imageBytes);
                        }

                        return Json(new {
                                            image64 = "data:image/jpeg;base64," + base64String
                                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
    }
}