using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace game.Models
{
    
    public static partial class MyJsInterop
    {
        //[JSImport("toDraw","game")]
        //public static partial void toDraw(int toDraw);

        [JSImport("globalThis.drawAll")]
        public static partial Task drawAll(string _toDraw);
    }
}
