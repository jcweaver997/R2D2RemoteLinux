using System;
using System.Threading;

namespace R2D2RemoteLinux
{
    public class R2D2RemoteLinux
    {
        private R2D2Connection con;
        private Joystick joy;
        private bool isRunning;
        
        public static void Main(string[] args)
        {
            R2D2RemoteLinux r2d2 = R2D2RemoteLinux();
            r2d2.Start();
        }

        public R2D2RemoteLinux()
        {
            con = new R2D2Connection(R2D2Connection.ConnectionType.Controller);
            Console.WriteLine("Select a joystick:");
            Joystick.PrintJoysticks();
            string devfile = Console.ReadLine().Trim();
            joy = new WeirdChineseController(devfile);
        }
        
        public void Start()
        {
            isRunning = true;

            new Thread(WaitForQuit).Start();
            
            con.Connect();
            while (isRunning)
            {
                Loop();
                Thread.Sleep(50);
            }
        }

        private void WaitForQuit()
        {
            string input = Console.ReadLine().Trim();
            while (!input.ToLower().Equals("q")&&!input.ToLower().Equals("quit"))
            {
                 input = Console.ReadLine().Trim();
            }

            isRunning = false;
        }

        private void Loop()
        {
            float[] tankVals = TranslateValuesFromGTAToTank(joy.GetThumbstickLeft().Y, joy.GetThumbstickRight().X);
            con.SendCommand(new R2D2Connection.Command(R2D2Connection.Commands.SetLeftDriveMotor,
                BitConverter.GetBytes(tankVals[0])));
            con.SendCommand(new R2D2Connection.Command(R2D2Connection.Commands.SetRightDriveMotor,
                BitConverter.GetBytes(tankVals[1])));
        }
        
        
        private float[] TranslateValuesFromGTAToTank(float throttle, float turn)
        {
            float[] ar = new float[2];
            if (throttle>.05f)
            {
                ar[0] = throttle;
                ar[1] = throttle;
                if (turn>0)
                {
                    ar[1] -= turn * .3f;
                }
                else
                {
                    ar[0] -= turn * .3f;
                }

            }
            else if (throttle < -.05f)
            {
                ar[0] = throttle;
                ar[1] = throttle;
                if (turn > 0)
                {
                    ar[1] += turn * .3f;
                }
                else
                {
                    ar[0] += turn * .3f;
                }

            }
            else
            {
                ar[0] = throttle;
                ar[1] = throttle;
                ar[0] += turn * .5f;
                ar[1] -= turn * .5f;
            }
            return ar;
        }
        
        
        
        
    }
}