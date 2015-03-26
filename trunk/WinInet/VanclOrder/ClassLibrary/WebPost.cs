﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
namespace ClassLibrary
{
    public class ClassHttpPost
    {
        public CookieContainer m_Cookie;
        public string GetHttpPage(string strUrl, string strEncode, string strType,
            string strCookie, string strData, string strReferer, string strAcceptType,
            string strExtendHead)
        {
            try
            {
                Stream stream = GetHttpPageSteam(strUrl, strEncode, strType, strCookie, strData, strReferer, strAcceptType, strExtendHead);
                if (stream == null)
                {
                    return "";
                }
                StreamReader sr = new StreamReader(stream, strEncode == "gb2312" ? Encoding.GetEncoding("gb2312") : Encoding.UTF8);
                string retStr = sr.ReadToEnd();
                sr.Close();
                return retStr; // UnZip(retStr);
            }
            catch (System.Exception e)
            {
                return "";
            }
            
        }

        public Stream GetHttpPageSteam(string strUrl, string strEncode, string strGetType,
            string strCookie, string strData, string strReferer, string strAcceptType,
            string strExtendHead)
        {
            try
            {
                HttpWebRequest m_Request = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                strGetType = strGetType.ToUpper();
                m_Request.Method = strGetType;
                m_Request.Timeout = 65000;
                if (strExtendHead == "")
                {
                    strExtendHead = "application/x-www-form-urlencoded";
                }
                m_Request.ContentType = strExtendHead;
                m_Request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; EmbeddedWB 14.52 from: http://www.bsalsa.com/ EmbeddedWB 14.52; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 1.1.4322)";
                m_Request.Accept = strAcceptType;// "*/*";
                m_Request.Referer = strReferer;
                m_Request.ServicePoint.Expect100Continue = false;
                m_Request.Headers.Add("Accept-Language", "zh-cn");
                m_Request.Headers.Add("Cache-Control", "no-cache");
               // m_Request.Headers.Add("Accept-Encoding", "gzip, deflate");
                if (m_Cookie == null || m_Cookie.Count == 0)
                {
                    m_Request.CookieContainer = new CookieContainer();
                    m_Cookie = m_Request.CookieContainer;
                }
                else
                {
                    m_Request.CookieContainer = m_Cookie;
                }

                if (strGetType != "GET")
                {
                    WriteRequestData(m_Request, EncodeParams(ref strData));
                }
                return m_Request.GetResponse().GetResponseStream();
            }
            catch (System.Exception ex)
            {
            	
            }
            return null;            
        }
       
        private void WriteRequestData(HttpWebRequest request, byte[] data)
        {
            request.ContentLength = data.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();
        }

        private byte[] EncodeParams(ref string strParams)
        {
            return Encoding.UTF8.GetBytes(strParams);
        }

        public static string UnZip(string value)
        {
            //Transform string into byte[]  
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }


            //Prepare for decompress  
            System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray);
            System.IO.Compression.GZipStream sr = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Decompress);


            //Reset variable to collect uncompressed result  
            byteArray = new byte[byteArray.Length];


            //Decompress  
            int rByte = sr.Read(byteArray, 0, byteArray.Length);


            //Transform byte[] unzip data to string  
            System.Text.StringBuilder sB = new System.Text.StringBuilder(rByte);
            //Read the number of bytes GZipStream red and do not a for each bytes in  
            //resultByteArray;  
            for (int i = 0; i < rByte; i++)
            {
                sB.Append((char)byteArray[i]);
            }
            sr.Close();
            ms.Close();
            sr.Dispose();
            ms.Dispose();
            return sB.ToString();
        }
    }
}
