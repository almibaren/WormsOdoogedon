﻿//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Security.Cryptography;
//using System.IO;
//using System;
//using System.Text;

//public class SimpleAES : MonoBehaviour {


//    private byte[] Key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
//    private byte[] Vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 252, 112, 79, 32, 114, 156 };
//    private ICryptoTransform EncryptorTransform, DecryptorTransform;
//    private System.Text.UTF8Encoding UTFEncoder;


//    public SimpleAES() {
//        RijndaelManaged rm = new RijndaelManaged();
//        EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
//        DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);
//        UTFEncoder = new System.Text.UTF8Encoding();
//    }

//    #region KeyVectorGenerator
//    /// -------------- Two Utility Methods (not used but may be useful) -----------
//    /// Generates an encryption key.
//    static public byte[] GenerateEncryptionKey() {
//        //Generate a Key.
//        RijndaelManaged rm = new RijndaelManaged();
//        rm.GenerateKey();
//        return rm.Key;
//    }
//    /// Generates a unique encryption vector
//    static public byte[] GenerateEncryptionVector() {
//        //Generate a Vector
//        RijndaelManaged rm = new RijndaelManaged();
//        rm.GenerateIV();
//        return rm.IV;
//    }
//    #endregion

//    public byte[] EncryptSingle(string TextValue) {

//        byte[] bytes = UTFEncoder.GetBytes(TextValue);
//        MemoryStream memoryStream = new MemoryStream();

//        #region Write the decrypted value to the encryption stream
//        CryptoStream cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
//        cs.Write(bytes, 0, bytes.Length);
//        cs.FlushFinalBlock();
//        #endregion

//        #region Read encrypted value back out of the stream
//        memoryStream.Position = 0;
//        byte[] encrypted = new byte[memoryStream.Length];
//        memoryStream.Read(encrypted, 0, encrypted.Length);
//        #endregion


//        cs.Close();
//        memoryStream.Close();
//        return encrypted;
//    }


//    public string DecryptSingle(byte[] EncryptedValue) {

//        #region Write the encrypted value to the decryption stream
//        MemoryStream encryptedStream = new MemoryStream();
//        CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
//        decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
//        //decryptStream.FlushFinalBlock();
//        #endregion

//        #region Read the decrypted value from the stream.
//        encryptedStream.Position = 0;
//        byte[] decryptedBytes = new byte[encryptedStream.Length];
//        encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
//        encryptedStream.Close();
//        #endregion

//        return UTFEncoder.GetString(decryptedBytes);
//    }
//    public string Encrypt(string message) {

//        string[] msg  = message.Split('|');
//        string msgFranken = ""; 
//        foreach (string s in msg) {
//            //string temp = Encoding.Unicode.GetString(EncryptSingle(s));
//            //Debug.Log("Este era temp->" + s);
//            //Debug.Log("Este es temp " + temp);

//            msgFranken += Encoding.UTF8.GetString(EncryptSingle(s)) + "|";
//        }
//        //string toReturn = msgFranken.Substring(0, msgFranken.Length - 1);
//        //Debug.Log("ESTE ES EL STRING ENTERO" + msgFranken);
//        return msgFranken;
//    }

//    public string Decrypt(string message) {

//        string[] msg = message.Split('|');
//        string msgDeFranken = "";
//        foreach(string s in msg) {
//            msgDeFranken += DecryptSingle(Encoding.Unicode.GetBytes(s)) + "|";
//        }
//        return msgDeFranken.Substring(0, msgDeFranken.Length - 1);
//    }

//    public byte[] StrToByteArray(string str) {

//        byte val;
//        byte[] byteArr = new byte[str.Length / 3];
//        int i = 0;
//        int j = 0;
//        do {
//            val = byte.Parse(str.Substring(i, 3));
//            byteArr[j++] = val;
//            i += 3;
//        }
//        while (i < str.Length);
//        return byteArr;
//    }

//    public string ByteArrToString(byte[] byteArr) {
//        byte val;
//        string tempStr = "";
//        for (int i = 0; i <= byteArr.GetUpperBound(0); i++) {
//            val = byteArr[i];
//            if (val < (byte)10)
//                tempStr += "00" + val.ToString();
//            else if (val < (byte)100)
//                tempStr += "0" + val.ToString();
//            else
//                tempStr += val.ToString();
//        }
//        return tempStr;
//    }

//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System;

public class SimpleAES : MonoBehaviour {

    // Change these keys
    private byte[] Key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };

    // a hardcoded IV should not be used for production AES-CBC code
    // IVs should be unpredictable per ciphertext
    private byte[] Vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 252, 112, 79, 32, 114, 156 };


    private ICryptoTransform EncryptorTransform, DecryptorTransform;
    private System.Text.UTF8Encoding UTFEncoder;

    public SimpleAES() {
        //This is our encryption method
        RijndaelManaged rm = new RijndaelManaged();

        //Create an encryptor and a decryptor using our encryption method, key, and vector.
        EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
        DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

        //Used to translate bytes to text and vice versa
        UTFEncoder = new System.Text.UTF8Encoding();
    }

    /// -------------- Two Utility Methods (not used but may be useful) -----------
    /// Generates an encryption key.
    static public byte[] GenerateEncryptionKey() {
        //Generate a Key.
        RijndaelManaged rm = new RijndaelManaged();
        rm.GenerateKey();
        return rm.Key;
    }

    /// Generates a unique encryption vector
    static public byte[] GenerateEncryptionVector() {
        //Generate a Vector
        RijndaelManaged rm = new RijndaelManaged();
        rm.GenerateIV();
        return rm.IV;
    }


    /// ----------- The commonly used methods ------------------------------    
    /// Encrypt some text and return a string suitable for passing in a URL.
    //public string EncryptToString(string TextValue) {
    //    return ByteArrToString(Encrypt(TextValue));
    //}

    /// Encrypt some text and return an encrypted byte array.
    public byte[] Encrypt(string TextValue) {
        //Translates our text value into a byte array.
        Byte[] bytes = UTFEncoder.GetBytes(TextValue);
        Byte[] bytesRellenado = new byte[0];
        if (bytes.Length <= 8) {
            bytesRellenado = new byte[8];
            for (int i = 0; i < bytes.Length; i++) {
                bytesRellenado[i] = 0;
            }
        } else if (bytes.Length <= 16) {
            bytesRellenado = new byte[16];
            for (int i = 0; i < bytes.Length; i++) {
                bytesRellenado[i] = 0;
            }
        } else if (bytes.Length <= 32) {
            bytesRellenado = new byte[32];
            for (int i = 0; i < bytes.Length; i++) {
                bytesRellenado[i] = 0;
            }
        } else if (bytes.Length <= 64) {
            bytesRellenado = new byte[64];
            for (int i = 0; i < bytes.Length; i++) {
                bytesRellenado[i] = 0;
            }
        } else if (bytes.Length <= 128) {
            bytesRellenado = new byte[128];
            for (int i = 0; i < bytes.Length; i++) {
                bytesRellenado[i] = 0;
            }
        } else if (bytes.Length <= 256) {
            bytesRellenado = new byte[256];
            for (int i = 0; i < bytes.Length; i++) {
                bytesRellenado[i] = 0;
            }
        }
        for (int i = 0; i < bytes.Length; i++) {
            bytesRellenado[i] = bytes[i];
        }

        //Used to stream the data in and out of the CryptoStream.
        MemoryStream memoryStream = new MemoryStream();

        /*
         * We will have to write the unencrypted bytes to the stream,
         * then read the encrypted result back from the stream.
         */
        #region Write the decrypted value to the encryption stream
        CryptoStream cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
        cs.Write(bytes, 0, bytes.Length);
        cs.FlushFinalBlock();
        #endregion

        #region Read encrypted value back out of the stream
        memoryStream.Position = 0;
        byte[] encrypted = new byte[memoryStream.Length];
        memoryStream.Read(encrypted, 0, encrypted.Length);
        #endregion

        //Clean up.
        cs.Close();
        memoryStream.Close();

        return encrypted;
    }

    /// The other side: Decryption methods
    //public string DecryptString(string EncryptedString) {
    //    return Decrypt(StrToByteArray(EncryptedString));
    //}

    /// Decryption when working with byte arrays.    
    public string Decrypt(byte[] EncryptedValue) {
        #region Write the encrypted value to the decryption stream
        MemoryStream encryptedStream = new MemoryStream();
        CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
        decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
        //decryptStream.FlushFinalBlock();
        #endregion

        #region Read the decrypted value from the stream.
        encryptedStream.Position = 0;
        Byte[] decryptedBytes = new Byte[encryptedStream.Length];
        encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
        encryptedStream.Close();
        #endregion
        return UTFEncoder.GetString(decryptedBytes);
    }

    /// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
    //      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
    //      return encoding.GetBytes(str);
    // However, this results in character values that cannot be passed in a URL.  So, instead, I just
    // lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
    public byte[] StrToByteArray(string str) {
        if (str.Length == 0)
            throw new Exception("Invalid string value in StrToByteArray");

        byte val;
        byte[] byteArr = new byte[str.Length / 3];
        int i = 0;
        int j = 0;
        do {
            val = byte.Parse(str.Substring(i, 3));
            byteArr[j++] = val;
            i += 3;
        }
        while (i < str.Length);
        return byteArr;
    }

    // Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction:
    //      System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
    //      return enc.GetString(byteArr);    
    public string ByteArrToString(byte[] byteArr) {
        byte val;
        string tempStr = "";
        for (int i = 0; i <= byteArr.GetUpperBound(0); i++) {
            val = byteArr[i];
            if (val < (byte)10)
                tempStr += "00" + val.ToString();
            else if (val < (byte)100)
                tempStr += "0" + val.ToString();
            else
                tempStr += val.ToString();
        }
        return tempStr;
    }
}