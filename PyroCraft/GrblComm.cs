using System;

partial class PyroCraft {
    private delegate bool GrblAbort();
    
    private bool Grbl_GetSync(GrblAbort callback = null) {
        int prevReadTimeout = serialPort1.ReadTimeout;
        try {
            serialPort1.ReadTimeout = 250;
            for (int i = 0; i < 16; i++) {
                if ((i % 4) == 0) {
                    serialPort1.BaudRate = 230400;
                } else if ((i % 4) == 3) {
                    serialPort1.BaudRate = 115200;
                }
                
                serialPort1.Write("\n\n");
                
                string resp = null;
                try {
                    for (;;) {
                        resp = serialPort1.ReadTo("\r\n");
                    }
                } catch (TimeoutException) {
                }
                
                if (callback != null) {
                    if (callback()) {
                        return false;
                    }
                }
                
                if (resp == null) {
                    continue;
                }
                if (resp != "ok") {
                    throw new Exception(resources.GetString("Grbl_InvalidResponse", culture));
                }
                
                return true;
            }
            
            throw new Exception(resources.GetString("Grbl_NotResponding", culture));
        } finally {
            serialPort1.ReadTimeout = prevReadTimeout;
        }
    }
}
