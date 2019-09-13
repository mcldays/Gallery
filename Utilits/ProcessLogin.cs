using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Utilits
{
    public static class ProcessLogin
    {
        public static string GetLogin(int number)
        {
            string[] Accounts = { };

            using (StreamReader st = new StreamReader("Accounts.txt"))
                Accounts = File.ReadAllLines("Accounts.txt");

            string[] Login = new string[Accounts.Length];
           
          

            
            for (int i = 0; i < Accounts.Length; ++i)
            {
                string flag = Accounts[i];
                
                string[] flagTwo = flag.Split(';');
                Login[i] = flagTwo[0].ToString();
             
            }

            return Login[number];
        }


        public static string GetPass(int number)
        {
            string[] Accounts = { };

            using (StreamReader st = new StreamReader("Accounts.txt"))
                Accounts = File.ReadAllLines("Accounts.txt");

          
            string[] Password = new string[Accounts.Length];
            


            for (int i = 0; i < Accounts.Length; ++i)
            {
                string flag = Accounts[i];

                string[] flagTwo = flag.Split(';');
                
                Password[i] = flagTwo[1].ToString();
            }

            return Password[number];
        }



    }
}

