using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nanka
{
    class Patch
    {
        /// <summary>
        /// ファイル書き換え処理
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="address">書換アドレス</param>
        /// <param name="oldData">書換前のデータ</param>
        /// <param name="newData">書換後のデータ</param>
        /// <returns>成功失敗</returns>
        public bool ReWrite(FileStream fs, ArrayList address, ArrayList oldData, ArrayList newData)
        {
            bool writeFlag = true;
            //書換前のデータが全て一致しているか調べる
            for (int i = 0; i < address.Count; i++)
            {
                if (false == Read1(fs, Convert.ToInt32(address[i]), Convert.ToByte(oldData[i])))
                {
                    writeFlag = false;
                }
            }
            //全て一致していたら書き換える
            if (writeFlag == true)
            {
                for (int i = 0; i < address.Count; i++)
                {
                    Write1(fs, Convert.ToInt32(address[i]), Convert.ToByte(newData[i]));
                }
            }
            return writeFlag;
        }
        /// <summary>
        /// 旧データ確認処理
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="address">書換アドレス</param>
        /// <param name="oldData">書換前のデータ</param>
        /// <returns>旧データと一致してればtrue</returns>
        protected bool Read1(FileStream fs, int address, byte oldData)
        {
            bool readFlag = false;
            fs.Position = address;
            //書き換え先のデータを読む
            int readData = fs.ReadByte();
            if (readData != -1)//読み込めなかったら書き換えない
            {
                if (oldData == (byte)readData)
                {
                    readFlag = true;
                }
            }
            return readFlag;
        }
        /// <summary>
        /// ファイル書き換え処理()
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="address">書換アドレス</param>
        /// <param name="newData">書換後のデータ</param>
        /// <returns>成功失敗</returns>
        protected void Write1(FileStream fs, int address, byte newData)
        {
            fs.Position = address;
            fs.WriteByte(newData);
        }
    }
}
