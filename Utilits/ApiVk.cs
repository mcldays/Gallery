using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;

namespace Gallery.Utilits
{
  class ApiVk
    {
        //public VkApi api;
        public  static VkApi api;
        public ApiVk()
        {
            var services = new ServiceCollection();
            services.AddAudioBypass();
            api = new VkApi(services);

            for (int i = 0; i < Utilits.ProcessLogin.GetLength(); i++)
            {
                var number = 1;
                try
                {
                    api.Authorize(new ApiAuthParams
                    {
                        Login = Utilits.ProcessLogin.GetLogin(number),
                        Password = Utilits.ProcessLogin.GetPass(number)
                    });
                    break;
                }
                catch
                {
                    number++;
                }
            }
        }

        public  VkApi Apireturn()
        {

            return api;
        }
    }
}
