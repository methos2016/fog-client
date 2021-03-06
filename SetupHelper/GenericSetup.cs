﻿/*
 * FOG Service : A computer management client for the FOG Project
 * Copyright (C) 2014-2015 FOG Project
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using Zazzles;
using Zazzles.Data;
using Zazzles.Middleware;

namespace FOG
{
    public static class GenericSetup
    {
        private const string LogName = "Installer";

        public static bool PinServerCert(string location, bool presetSettings = false)
        {

            if (!presetSettings)
                Settings.SetPath(Path.Combine(location, "settings.json"));

            Configuration.GetAndSetServerAddress();
            Configuration.ServerAddress = Configuration.ServerAddress.Replace("https://", "http://");

            return PinServerCertPreset(location);
        }

        public static bool PinServerCert(string address, string webroot, string location)
        {
            Configuration.ServerAddress = "http://" + address + webroot;
            return PinServerCertPreset(location);
        }


        private static bool PinServerCertPreset(string location)
        {
            try
            {
                var keyPath = Path.Combine(location, "ca.cert.der");

                var cert = RSA.ServerCertificate();
                if (cert != null) UnpinServerCert();


                var downloaded = Communication.DownloadFile("/management/other/ca.cert.der", keyPath);
                if (!downloaded)
                    return false;

                cert = new X509Certificate2(keyPath);

                return RSA.InjectCA(cert);
            }
            catch (Exception ex)
            {
                Log.Error(LogName, "Could not pin server CA");
                Log.Error(LogName, ex);
                throw;
            }
        }

        public static bool SaveSettings(string https, string usetray, string webaddress, string webroot,
            string company, string rootLog, string version, string location)
        {
            var filePath = Path.Combine(location, "settings.json");
            try
            {

                if (File.Exists(filePath))
                {
                    var settings = JObject.Parse(File.ReadAllText(filePath));
                    settings["Version"] = version;
                    File.WriteAllText(filePath, settings.ToString());
                }
                else
                {
                    var settings = new JObject
                    {
                        {"HTTPS", https},
                        {"Tray", usetray},
                        {"Server", webaddress},
                        {"WebRoot", webroot},
                        {"Version", version},
                        {"Company", company},
                        {"RootLog", rootLog}
                    };
                    File.WriteAllText(filePath, settings.ToString());
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(LogName, "Could not save settings");
                Log.Error(LogName, ex);
                throw;
            }
        }

        public static bool UnpinServerCert()
        {
            var cert = RSA.ServerCertificate();
            if (cert == null) return false;

            try
            {
                var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Remove(cert);
                store.Close();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(LogName, "Could unpin server CA cert");
                Log.Error(LogName, ex);
                throw;
            }
        }

        public static bool InstallFOGCert(string location)
        {
            try
            {
                var cert = new X509Certificate2(location);
                var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);

                store.Close();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(LogName, "Unable to install FOG CA cert");
                Log.Error(LogName, ex);
                throw;
            }
        }

        public static void UninstallFOGCert()
        {
            var cert = new X509Certificate2();
            try
            {
                X509Certificate2 CAroot = null;
                var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                var cers = store.Certificates.Find(X509FindType.FindBySubjectName, "FOG Project", true);

                if (cers.Count > 0)
                {
                    CAroot = cers[0];
                }
                store.Close();

                cert = CAroot;
            }
            catch (Exception ex)
            {
                Log.Error(LogName, "Could not find FOG Project CA cert");
                Log.Error(LogName, ex);
                throw;
            }

            if (cert == null) return;

            try
            {
                var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Remove(cert);
                store.Close();
            }
            catch (Exception ex)
            {
                Log.Error(LogName, "Could not remove FOG Project CA cert");
                Log.Error(LogName, ex);
                throw;
            }

        }
    }
}