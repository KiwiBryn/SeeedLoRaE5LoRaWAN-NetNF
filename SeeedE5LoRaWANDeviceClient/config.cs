﻿//---------------------------------------------------------------------------------
// Copyright (c) May 2021, devMobile Software
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// must have one of OTAA or ABP in nfprroj file
//---------------------------------------------------------------------------------
namespace devMobile.IoT.SeeedE5LoRaWANDeviceClient
{
   using System;

   public class Config
   {
#if OTAA
      public const string AppEui = "..:..:..:..:..:..:..:..";  
      public const string AppKey = "01234567890ABCDEF01234567890ABCD";
#endif
#if ABP
      public const string DevAddress = "00:00:00:00";
      public const string NwksKey = "01234567890ABCDEF01234567890ABCD";
      public const string AppsKey = "01234567890ABCDEF01234567890ABCD";
#endif   
   }
}
