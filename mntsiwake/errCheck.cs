using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace mntsiwake
{
    class errCheck
    {
        public struct Errtbl
        {
            public int Count;           //エラー件数
            public int DenNo;           //エラー伝票番号
            public int LINE;            //エラー行番号
            public string Field;        //エラーフィールド
            public string Data;         //エラーデータ
            public string Notes;        //エラー備考
            public string DpPos;        //MultiRowのセル名
        }

        //エラー情報配列のインスタンスを作成
        public Errtbl[] eTbl = new Errtbl[1];

        //エラー件数
        public int ErrCnt = 0;

        //コンストラクタ　：　エラー件数をゼロとする
        public errCheck()
        {
            eTbl[0].Count = 0;
        }

        ///--------------------------------------------------
        /// <summary>
        ///     結合枚数のチェック </summary>
        /// <param name="iX">
        ///     伝票配列データの添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///--------------------------------------------------
        public void ChkCombineNEW(int iX, Entity.InputRecord[] st)
        {
            //1行目に複数枚チェックが入っていたらNG
            if ((iX == 0) && (st[iX].Head.FukusuChk == "1"))
            {
                //エラーテーブルに値を格納
                ErrCnt++;
                ErrorTableSet(iX,0,"結合",string.Empty,"先頭伝票に複数チェックが入っています。",MultiRow.DP_FUKU);
                return;
            }

            //複数チェックなし
            if (st[iX].Head.FukusuChk == "0")
            {
                global.pblMaisu = 1;
            }
            //複数チェックあり
            else
            {
                global.pblMaisu ++;
            }

            //結合可能枚数を越えた時
            if (global.pblMaisu > global.pblCombineMax)
            {
                //エラーテーブルに値を確保
                ErrCnt++;
                ErrorTableSet(iX,0,"結合",global.pblMaisu.ToString(),"最大結合枚数を超えています","");
            }
        }


        ///--------------------------------------------------
        /// <summary>
        ///     結合行数チェック </summary>
        /// <param name="iX">
        ///     伝票配列データの添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///--------------------------------------------------
        public void ChkCombineItem(int iX, Entity.InputRecord[] st)
        {
            int ItemLimit = 0;
    
            //判定方法追加「バージョンで判定」 (v6.0対応)--
            //////if (int.Parse(company.gsVersion) < 93) //(Version 1.0 or 2000)
            //////{
            //////    ItemLimit = global.MAX2000;
            //////}
            //////else
            //////{
            //////    ItemLimit = global.MAX21; //(Version 2.0)
            //////}

            ItemLimit = global.MAX21; //(Version 2.0)

            //最大行数を越えた時
            if (ChkVersion(iX,st) > ItemLimit)
            {
                //エラーテーブルに値を格納
                ErrCnt++;
                ErrorTableSet(iX,0,"行数",global.pblItem.ToString(),"最大処理行数を超えています",MultiRow.DP_FUKU);
            }
        }

        /// <summary>
        /// 結合された明細行をカウントする
        /// </summary>
        /// <param name="Cnt">伝票配列データ添え字</param>
        /// <param name="st">伝票配列データ</param>
        /// <returns></returns>
        private int ChkVersion(int Cnt, Entity.InputRecord[] st)
        {    
            //複数チェックなし
            if (st[Cnt].Head.FukusuChk == "0")
            {
                //カウント初期化
                global.pblItem = 0;
            }

            for (int i = 0; i < st[Cnt].Gyou.Length; i++)
            {
                //取消行はカウントしない
                if (st[Cnt].Gyou[i].Torikeshi == "0")
                {
                    //空白行でなければ・・・
                    if ((st[Cnt].Gyou[i].Kari.Kamoku != string.Empty) ||
                        (st[Cnt].Gyou[i].Kashi.Kamoku != string.Empty) ||
                        (st[Cnt].Gyou[i].CopyChk != string.Empty) ||
                        (st[Cnt].Gyou[i].Tekiyou.Trim() != string.Empty))
                    {
                        //カウントを足す
                        global.pblItem++;
                    }
                }
            }

            return global.pblItem;
        }

        ///--------------------------------------------------
        /// <summary>
        ///     結合日付チェック </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///--------------------------------------------------
        public void ChkCombineDateNEW(int iX, Entity.InputRecord[] st)
        {
            //先頭伝票はネグる
            if (iX > 0)
            {
                //複数チェックあり
                if (st[iX].Head.FukusuChk != "0")
                {
                    //前伝票と日付が異なっていた場合エラー
                    if ((st[iX].Head.Year != st[iX - 1].Head.Year) || 
                        (st[iX].Head.Month != st[iX - 1].Head.Month) || 
                        (st[iX].Head.Day != st[iX - 1].Head.Day))
                    {
            
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX,0,"結合",
                                      company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                                      "結合伝票で日付が異なっています。",MultiRow.DP_DENYEAR);
                    }
                }
            }
        }

        ///--------------------------------------------------
        /// <summary>
        ///     伝票No.チェック </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///--------------------------------------------------
        public void ChkCombineDenNoNEW(int iX, Entity.InputRecord[] st)
        {
            // 伝票No.不正チェック　　ＮＧ，数字以外又はマイナスはNG
            if (st[iX].Head.DenNo == "NG")
            {
                // エラーテーブルに値を格納
                ErrCnt++;
                ErrorTableSet(iX, 0, "№", st[iX].Head.DenNo, "ＮＧ伝票です。", MultiRow.DP_DENNO);
            }
            else if (utility.NumericCheck(st[iX].Head.DenNo) == false)
            {
                // エラーテーブルに値を格納
                ErrCnt++;
                ErrorTableSet(iX, 0, "№", st[iX].Head.DenNo, "伝票№が不正です。", MultiRow.DP_DENNO);
            }
            else if (int.Parse(st[iX].Head.DenNo) < 0)
            {
                // エラーテーブルに値を格納
                ErrCnt++;
                ErrorTableSet(iX, 0, "№", st[iX].Head.DenNo, "伝票№が不正です。", MultiRow.DP_DENNO);
            }
            else if (int.Parse(st[iX].Head.DenNo) == 0)
            {
                // エラーテーブルに値を格納
                ErrCnt++;
                ErrorTableSet(iX, 0, "№", st[iX].Head.DenNo, "伝票№が不正です。", MultiRow.DP_DENNO);
            }

            ////先頭伝票はネグる
            //if (iX > 0)
            //{
            //    //複数チェックあり
            //    if (st[iX].Head.FukusuChk != "0")
            //    {
            //        //前伝票と伝票No.が異なっていた場合エラー
            //        if (st[iX].Head.DenNo != st[iX - 1].Head.DenNo)
            //        {
            //            //エラーテーブルに値を格納
            //            ErrCnt++;
            //            ErrorTableSet(iX, 0, "№", st[iX].Head.DenNo, "結合伝票で伝票№が異なっています。", MultiRow.DP_DENNO);
            //        }
            //    }
            //}
        }

        ///--------------------------------------------------------
        /// <summary>
        ///     結合決算チェック</summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///--------------------------------------------------------
        public void ChkCombineKessanNEW(int iX, Entity.InputRecord[] st)
        {
            //先頭伝票はネグる
            if (iX > 0)
            {
                //複数チェックあり
                if (st[iX].Head.FukusuChk != "0")
                {
                    //前伝票と決算区分が異なっていた場合エラー
                    if (st[iX].Head.Kessan != st[iX - 1].Head.Kessan)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, 0, "決算",st[iX].Head.Kessan, "結合伝票に通常月と決算月整理仕訳が混在しています。", MultiRow.DP_KESSAN);
                    }
                }
            }
        }

        ///--------------------------------------------------------------------
        /// <summary>
        ///     存在する日付かチェック() </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        /// <returns>
        ///     </returns>
        ///--------------------------------------------------------------------
        public Boolean ChkDateNEW(int iX, Entity.InputRecord[] st)
        {
            if ((ChkDateIndi(st[iX].Head.Year, st[iX].Head.Month, st[iX].Head.Day, company.Hosei) == false))
            {
                //エラーテーブルに値を格納
                ErrCnt++;
                ErrorTableSet(iX, 0, "日付", company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                    "存在しない日付です。", MultiRow.DP_DENYEAR);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 日付チェック
        /// </summary>
        /// <param name="Year">年</param>
        /// <param name="Month">月</param>
        /// <param name="Day">日</param>
        /// <param name="Hosei">補正</param>
        /// <returns>存在する日付：true、存在しない日付：false</returns>
        public Boolean ChkDateIndi(String Year, String Month, String Day, String Hosei)
        {
            int wrkADYear;

            //空欄はNG
            if ((Year == string.Empty) || (Month == string.Empty) || (Day == string.Empty)) return false;

            //数字以外、2桁以上はNG
            if (utility.NumericCheck(Year) == false || Year.Length > 2 || 
                utility.NumericCheck(Month) == false || Month.Length > 2 || 
                utility.NumericCheck(Day) == false || Day.Length > 2) return false;
            
            //0年NG
            if (int.Parse(Year) == 0) return false;

            //西暦を求める
            //////if (Hosei != "0") //和暦のとき
            if (global.pblReki == global.RWAREKI) //和暦のとき
            {
                wrkADYear = int.Parse(Year) + int.Parse(Hosei);
            }
            else if (int.Parse(Year) < 70)
            {
                    wrkADYear = int.Parse(Year) + 2000;
            }
            else
            {
                    wrkADYear = int.Parse(Year) + 1900;
            }
      
            //日付変換可能か？
            DateTime r = new DateTime();
            if (DateTime.TryParse(wrkADYear.ToString() + "/" + Month + "/" + Day, out r) == false) return false;

            return true;
        }

        ///-------------------------------------------------------
        /// <summary>
        ///     決算日付チェック </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        /// <returns>
        ///     </returns>
        ///-------------------------------------------------------
        public Boolean ChkDateKessanNEW(int iX, Entity.InputRecord[] st)
        {
            Boolean wrkRetValue = true;
            DateTime mDate;
            DateTime mDate2;
            DateTime mDate3;
            
            //決算チェックがあり、中間期決算をしない場合
            if (st[iX].Head.Kessan == "1" && company.Middle == global.FLGOFF)
            {
                //決算期間のチェック
                if (ChkKessanIndi(st[iX].Head.Year, st[iX].Head.Month,　company.ToYear, company.ToMonth) == false)
                   wrkRetValue = false;
            }
            //決算チェックがあり、中間期決算を行う場合
            else if (st[iX].Head.Kessan == "1" && company.Middle == global.FLGON)
            {
                // 中間決算月を取得
                mDate = company.fromDate.AddMonths(5); 

                // 中間期決算、決算期間のチェック
                if (ChkKessanIndi(st[iX].Head.Year, st[iX].Head.Month, mDate.Year.ToString(), mDate.Month.ToString()) == false &&
                    ChkKessanIndi(st[iX].Head.Year, st[iX].Head.Month, company.ToYear, company.ToMonth) == false)
                    wrkRetValue = false;
            }
            //決算チェックがあり、四半期決算を行う場合
            else if (st[iX].Head.Kessan == "1" && company.Middle == global.FLGON_2)
            {
                // 四半期決算月を取得
                mDate = company.fromDate.AddMonths(2);
                mDate2 = company.fromDate.AddMonths(5);
                mDate3 = company.fromDate.AddMonths(8); 

                //四半期決算、決算期間のチェック
                if (ChkKessanIndi(st[iX].Head.Year, st[iX].Head.Month, mDate.Year.ToString(), mDate.Month.ToString()) == false &&
                    ChkKessanIndi(st[iX].Head.Year, st[iX].Head.Month, mDate2.Year.ToString(), mDate2.Month.ToString()) == false &&
                    ChkKessanIndi(st[iX].Head.Year, st[iX].Head.Month, mDate3.Year.ToString(), mDate3.Month.ToString()) == false &&
                    ChkKessanIndi(st[iX].Head.Year, st[iX].Head.Month, company.ToYear, company.ToMonth) == false)
                    wrkRetValue = false;
            }
        
            if (wrkRetValue == false)
            {
                //エラーテーブルに値を格納
                ErrCnt++;
                ErrorTableSet(iX, 0, "日付",company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                    "決算月ではありません。", MultiRow.DP_DENYEAR);
            }

            return wrkRetValue;
        }

        /// <summary>
        /// 会計期間チェック
        /// </summary>
        /// <param name="iX">伝票配列データ添え字</param>
        /// <param name="st">伝票配列データ</param>
        /// <returns></returns>
        public Boolean ChkDateKikanNEW(int iX, Entity.InputRecord[] st)
        {
            if (ChkKikanIndi(st[iX].Head.Year, st[iX].Head.Month, st[iX].Head.Day,
                              company.FromYear, company.FromMonth, company.FromDay,
                              company.ToYear, company.ToMonth, company.ToDay) == false)
            {
                //エラーテーブルに値を格納
                ErrCnt++;
                ErrorTableSet(iX, 0, "日付",company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                    "会計期間外の日付です。", MultiRow.DP_DENYEAR);
                return false;
            }
    
            return true;
        }

        /// <summary>
        /// 日付入力範囲チェック
        /// </summary>
        /// <param name="iX">伝票配列データ添え字</param>
        /// <param name="st">伝票配列データ</param>
        public void ChkDateLimitNEW(int iX, Entity.InputRecord[] st)
        {
            //決算チェックがない場合
            if (st[iX].Head.Kessan != "1")
            {
                //通常入力禁止の場合はNG
                if (Limit.LimitKikan.Flag == false)
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX, 0, "日付",company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                    "入力範囲外の日付です。", MultiRow.DP_DENYEAR);
                }
            
                //日付のチェック
                else if (ChkKikanIndi(st[iX].Head.Year, st[iX].Head.Month, st[iX].Head.Day,
                         Limit.LimitKikan.FromYear,Limit.LimitKikan.FromMonth,Limit.LimitKikan.FromDay,
                         Limit.LimitKikan.ToYear,Limit.LimitKikan.ToMonth,Limit.LimitKikan.ToDay) == false)
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX, 0, "日付",company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                    "入力範囲外の日付です。", MultiRow.DP_DENYEAR);
                }
            }
            //決算チェックがあり、中間期決算をしない場合
            else if (st[iX].Head.Kessan == "1" && company.Middle == global.FLGOFF)
            {
                //決算禁止の場合はNG
                if (Limit.KessanDate.Flag == false)
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX, 0, "日付",company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                    "入力範囲外の日付です。", MultiRow.DP_DENYEAR);
                }
                //決算期間のチェック
                else if (ChkKikanIndi(st[iX].Head.Year, st[iX].Head.Month, st[iX].Head.Day,
                         Limit.KessanDate.FromYear,Limit.KessanDate.FromMonth,Limit.KessanDate.FromDay,
                         Limit.KessanDate.ToYear,Limit.KessanDate.ToMonth,Limit.KessanDate.ToDay) == false)
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX, 0, "日付",company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                    "入力範囲外の日付です。", MultiRow.DP_DENYEAR);            
                }
            }
            //決算チェックがあり、中間期決算を行う場合
            else if (st[iX].Head.Kessan == "1" && company.Middle == global.FLGON)
            {
                //中間期決算、決算ともに禁止の場合はNG
                if (Limit.MidKessanDate.Flag == false && Limit.KessanDate.Flag == false)
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX, 0, "日付",company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                    "入力範囲外の日付です。", MultiRow.DP_DENYEAR);   
                }
            
                //中間期決算のみ禁止
                else if (Limit.MidKessanDate.Flag == false && Limit.KessanDate.Flag == true)
                {
                    if (ChkKikanIndi(st[iX].Head.Year, st[iX].Head.Month, st[iX].Head.Day,
                        Limit.KessanDate.FromYear,Limit.KessanDate.FromMonth,Limit.KessanDate.FromDay,
                        Limit.KessanDate.ToYear,Limit.KessanDate.ToMonth,Limit.KessanDate.ToDay) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, 0, "日付",company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                        "入力範囲外の日付です。", MultiRow.DP_DENYEAR);             
                    }
                }
                //決算のみ禁止
                else if (Limit.MidKessanDate.Flag == true && Limit.KessanDate.Flag == false)
                {
                    if (ChkKikanIndi(st[iX].Head.Year, st[iX].Head.Month, st[iX].Head.Day,
                                    Limit.MidKessanDate.FromYear,Limit.MidKessanDate.FromMonth,Limit.MidKessanDate.FromDay,
                                    Limit.MidKessanDate.ToYear,Limit.MidKessanDate.ToMonth,Limit.MidKessanDate.ToDay) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, 0, "日付",company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                        "入力範囲外の日付です。", MultiRow.DP_DENYEAR); 
                    }
                }
                //中間期決算、決算ともに許可
                else if (Limit.MidKessanDate.Flag == true && Limit.KessanDate.Flag == true)
                {
                    if ((ChkKikanIndi(st[iX].Head.Year, st[iX].Head.Month, st[iX].Head.Day,
                                     Limit.MidKessanDate.FromYear,Limit.MidKessanDate.FromMonth,Limit.MidKessanDate.FromDay,
                                     Limit.MidKessanDate.ToYear,Limit.MidKessanDate.ToMonth,Limit.MidKessanDate.ToDay) == false) && 
                       (ChkKikanIndi(st[iX].Head.Year, st[iX].Head.Month, st[iX].Head.Day,
                                     Limit.KessanDate.FromYear,Limit.KessanDate.FromMonth,Limit.KessanDate.FromDay,
                                     Limit.KessanDate.ToYear,Limit.KessanDate.ToMonth,Limit.KessanDate.ToDay) == false))
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, 0, "日付", company.Reki + st[iX].Head.Year + "年" + st[iX].Head.Month + "月" + st[iX].Head.Day + "日",
                        "入力範囲外の日付です。", MultiRow.DP_DENYEAR); 
                    }
                }
            }
        }


        /// <summary>
        /// 日付入力範囲チェック
        /// </summary>
        /// <param name="Year">対象年</param>
        /// <param name="Month">対象月</param>
        /// <param name="Day">対象日</param>
        /// <param name="fYear">開始年</param>
        /// <param name="fMonth">開始月</param>
        /// <param name="fDay">開始日</param>
        /// <param name="tYear">終了年</param>
        /// <param name="tMonth">終了月</param>
        /// <param name="tDay">終了日</param>
        /// <returns>範囲内：true、範囲外：false</returns>
        public Boolean ChkKikanIndi(string Year, string Month, string Day,
                                    string fYear, string fMonth, string fDay,
                                    string tYear, string tMonth, string tDay)
        {
            string wrkYear;
            DateTime sDate;
            DateTime fDate;
            DateTime tDate;

            fYear = fYear.Trim();
            fMonth = fMonth.Trim();
            fDay = fDay.Trim();
            tYear = tYear.Trim();
            tMonth = tMonth.Trim();
            tDay = tDay.Trim();
        
            //和暦
            if (global.pblReki == global.RWAREKI)
            {
                wrkYear = (int.Parse(Year) + int.Parse(company.Hosei)).ToString();
            }
            //西暦
            else
            {
                if (int.Parse(Year) < 70)
                {
                    wrkYear = (int.Parse(Year) + 2000).ToString();
                }
                else
                {
                    wrkYear = (int.Parse(Year) + 1900).ToString();
                }
            }
    
            DateTime.TryParse(wrkYear + "/" + Month + "/" + Day,out sDate);
            DateTime.TryParse(fYear + "/" + fMonth + "/" + fDay,out fDate);
            DateTime.TryParse(tYear + "/" + tMonth + "/" + tDay,out tDate);
            
            //Fromより前のときはNG
            if (sDate < fDate) return false;

            //Toより後のとき
            if (sDate > tDate) return false;

            return true;
        }

        /// <summary>
        /// 決算該当年月チェック
        /// </summary>
        /// <param name="Year">対象年</param>
        /// <param name="Month">対象月</param>
        /// <param name="fYear">開始年</param>
        /// <param name="fMonth">開始月</param>
        /// <returns>該当：true、非該当：false</returns>
        public Boolean ChkKessanIndi(string Year, string Month, string fYear, string fMonth)
        {
            string wrkYear;

            fYear = fYear.Trim();
            fMonth = fMonth.Trim();

            //和暦
            if (global.pblReki == global.RWAREKI)
            {
                wrkYear = (int.Parse(Year) + int.Parse(company.Hosei)).ToString();
            }
            //西暦
            else
            {
                if (int.Parse(Year) < 70)
                {
                    wrkYear = (int.Parse(Year) + 2000).ToString();
                }
                else
                {
                    wrkYear = (int.Parse(Year) + 1900).ToString();
                }
            }

            //年月が一致しないときNG
            if (wrkYear != fYear || int.Parse(Month) != int.Parse(fMonth)) return false;

            return true;
        }

        /// <summary>
        /// 入力不備チェック
        /// </summary>
        /// <param name="iX">伝票配列データ添え字</param>
        /// <param name="st">伝票配列データ</param>
        public void ChkDataPoorNEW(int iX, Entity.InputRecord[] st)
        {
            //行ループ
            for (int i = 0; i < st[iX].Gyou.Length; i++)
			{
                //取消行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    //部門科目が空欄のとき 
                    //if ((global.pblBumonFlg == true && st[iX].Gyou[i].Kari.Bumon == string.Empty) && 
                    //    (st[iX].Gyou[i].Kari.Kamoku != string.Empty))
                    //{
                    //    //エラーテーブルに値を格納
                    //    ErrCnt++;
                    //    ErrorTableSet(iX, i, "借", "部門未登録","データに不備があります。", MultiRow.DP_KARI_CODEB); 
                    //}
            
                    //借方科目が空欄のとき
                    if (st[iX].Gyou[i].Kari.Kamoku == string.Empty)
                    {
                        //他の借方欄に何か記入されていたらNG
                        if ((st[iX].Gyou[i].Kari.Bumon != string.Empty) || 
                            (st[iX].Gyou[i].Kari.Hojo != string.Empty) || 
                            (st[iX].Gyou[i].Kari.Kin != string.Empty) || 
                            (st[iX].Gyou[i].Kari.TaxMas != string.Empty) || 
                            (st[iX].Gyou[i].Kari.TaxKbn != string.Empty) ||
                            (st[iX].Gyou[i].Kari.JigyoKbn != string.Empty))
                        {
                            //エラーテーブルに値を格納
                            ErrCnt++;
                            ErrorTableSet(iX, i, "借", "勘定科目未登録","データに不備があります。", MultiRow.DP_KARI_CODE);
                        }
                    }
                    //借方科目が記入されているとき
                    else
                    {
                        //金額欄が空欄のときNG
                        if (st[iX].Gyou[i].Kari.Kin == string.Empty)
                        {
                            //エラーテーブルに値を格納
                            ErrCnt++;
                            ErrorTableSet(iX, i, "借", "金額未登録","データに不備があります。", MultiRow.DP_KARI_KIN);
                        }
                    }
        
                    //部門科目が空欄のとき
                    //if ((global.pblBumonFlg == true && st[iX].Gyou[i].Kashi.Bumon == string.Empty) && 
                    //    (st[iX].Gyou[i].Kashi.Kamoku != string.Empty))
                    //{
                    //        //エラーテーブルに値を格納
                    //        ErrCnt++;
                    //        ErrorTableSet(iX, i, "貸", "部門未登録","データに不備があります。", MultiRow.DP_KASHI_CODEB);
                    //}
            
                    //貸方科目が空欄のとき
                    if (st[iX].Gyou[i].Kashi.Kamoku == string.Empty)
                    {
                        //他の貸方欄（摘要以外）に何か記入されていたらNG
                        if ((st[iX].Gyou[i].Kashi.Bumon != string.Empty) || 
                            (st[iX].Gyou[i].Kashi.Hojo != string.Empty) || 
                            (st[iX].Gyou[i].Kashi.Kin != string.Empty) || 
                            (st[iX].Gyou[i].Kashi.TaxMas != string.Empty) || 
                            (st[iX].Gyou[i].Kashi.TaxKbn != string.Empty) || 
                            (st[iX].Gyou[i].Kashi.JigyoKbn != string.Empty))
                        {
                            //エラーテーブルに値を格納
                            ErrCnt++;
                            ErrorTableSet(iX, i, "貸", "勘定科目未登録","データに不備があります。", MultiRow.DP_KASHI_CODE);
                        }
                    }
                    //貸方科目が記入されているとき
                    else
                    {
                        //金額欄が空欄のときNG
                        if (st[iX].Gyou[i].Kashi.Kin == string.Empty )
                        {
                            //エラーテーブルに値を格納
                            ErrCnt++;
                            ErrorTableSet(iX, i, "貸", "金額未登録","データに不備があります。", MultiRow.DP_KASHI_KIN);
                        }
                    }
                }
            }
        }

        ///--------------------------------------------------
        /// <summary>
        ///     科目コードチェック：勘定奉行i10 2017/09/03</summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///--------------------------------------------------
        public void ChkKamokuNEW(int iX, Entity.InputRecord[] st)
        {
            //行ループ
            for (int i = 0; i < st[iX].Gyou.Length; i++)
            {                
                //取消行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    //借方
                    if ((ChkKamokuIndi(st[iX].Gyou[i].Kari.Kamoku) == false))
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "借", st[iX].Gyou[i].Kari.Kamoku,"不正な勘定科目コードです。", MultiRow.DP_KARI_CODE);
                    }
                
                    //貸方
                    if ((ChkKamokuIndi(st[iX].Gyou[i].Kashi.Kamoku) == false))
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "貸", st[iX].Gyou[i].Kashi.Kamoku,"不正な勘定科目コードです。", MultiRow.DP_KASHI_CODE);
                    }
                }
            }
        }

        ///--------------------------------------------------------------
        /// <summary>
        ///     補助コードチェック </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///--------------------------------------------------------------
        public void ChkHojoNEW(int iX, Entity.InputRecord[] st)
        {
            string KanjoCode = string.Empty;

            //行ループ
            for (int i = 0; i < st[iX].Gyou.Length; i++)
            {
                //取消行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    //借方 /////////////////////////////////////////////////////////////////////////////////////////

                    //勘定科目取得
                    if (utility.NumericCheck(st[iX].Gyou[i].Kari.Kamoku.Trim()))
                    {
                        KanjoCode = string.Format("{0:D10}", int.Parse(st[iX].Gyou[i].Kari.Kamoku.Trim()));
                    }
                    else
                    {
                        KanjoCode = st[iX].Gyou[i].Kari.Kamoku.Trim();
                    }

                    //エラーチェック
                    if (ChkHojoIndi(st[iX].Gyou[i].Kari.Hojo.Trim(), KanjoCode) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "借", st[iX].Gyou[i].Kari.Hojo,"不正な補助科目コードです。", MultiRow.DP_KARI_CODEH); 
                    }

                    //貸方 /////////////////////////////////////////////////////////////////////////////////////////

                    //勘定科目取得
                    if (utility.NumericCheck(st[iX].Gyou[i].Kashi.Kamoku.Trim()))
                    {
                        KanjoCode = string.Format("{0:D10}", int.Parse(st[iX].Gyou[i].Kashi.Kamoku.Trim()));
                    }
                    else
                    {
                        KanjoCode = st[iX].Gyou[i].Kashi.Kamoku.Trim();
                    }

                    //エラーチェック
                    if (ChkHojoIndi(st[iX].Gyou[i].Kashi.Hojo.Trim(), KanjoCode) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "貸", st[iX].Gyou[i].Kashi.Hojo, "不正な補助科目コードです。", MultiRow.DP_KASHI_CODEH); 
                    }
                }
            }
        }

        ///------------------------------------------------------
        /// <summary>
        ///     部門コードチェック：勘定奉行i10 2017/09/03 </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///------------------------------------------------------
        public void ChkBumonNEW(int iX, Entity.InputRecord[] st)
        {
            //行ループ
            for (int i = 0; i < st[iX].Gyou.Length; i++)
            {
                //取り消し行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    //借方
                    if (ChkBumonIndi(st[iX].Gyou[i].Kari.Bumon.Trim()) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "借", st[iX].Gyou[i].Kari.Bumon,"不正な部門コードです。", MultiRow.DP_KARI_CODEB); 
                    }

                    //貸方
                    if (ChkBumonIndi(st[iX].Gyou[i].Kashi.Bumon.Trim()) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "貸", st[iX].Gyou[i].Kashi.Bumon,"不正な部門コードです。", MultiRow.DP_KASHI_CODEB);
                    }

                    // 勘定科目との関連チェック・借方
                    if (st[iX].Gyou[i].Kari.Kamoku.Trim() != string.Empty)
                    {
                        if (utility.NumericCheck(st[iX].Gyou[i].Kari.Kamoku.Trim()))
                        {
                            int km = int.Parse(st[iX].Gyou[i].Kari.Kamoku.Trim());
                            if (100 <= km && km <= 399)
                            {
                                if (st[iX].Gyou[i].Kari.Bumon.Trim() != string.Empty)
                                {
                                    //エラーテーブルに値を格納
                                    ErrCnt++;
                                    ErrorTableSet(iX, i, "借", st[iX].Gyou[i].Kari.Bumon, "不要な枝番です。", MultiRow.DP_KARI_CODEB); 
                                }
                            }
                            else if (km >= 400)
                            {
                                if (st[iX].Gyou[i].Kari.Bumon.Trim() == string.Empty)
                                {
                                    //エラーテーブルに値を格納
                                    ErrCnt++;
                                    ErrorTableSet(iX, i, "借", st[iX].Gyou[i].Kari.Bumon, "枝番が未登録です。", MultiRow.DP_KARI_CODEB);
                                }
                            }
                        }
                    }
                    
                    // 勘定科目との関連チェック・貸方
                    if (st[iX].Gyou[i].Kashi.Kamoku.Trim() != string.Empty)
                    {
                        if (utility.NumericCheck(st[iX].Gyou[i].Kashi.Kamoku.Trim()))
                        {
                            int km = int.Parse(st[iX].Gyou[i].Kashi.Kamoku.Trim());
                            if (100 <= km && km <= 399)
                            {
                                if (st[iX].Gyou[i].Kashi.Bumon.Trim() != string.Empty)
                                {
                                    //エラーテーブルに値を格納
                                    ErrCnt++;
                                    ErrorTableSet(iX, i, "貸", st[iX].Gyou[i].Kashi.Bumon, "不要な枝番です。", MultiRow.DP_KASHI_CODEB);
                                }
                            }
                            else if (km >= 400)
                            {
                                if (st[iX].Gyou[i].Kashi.Bumon.Trim() == string.Empty)
                                {
                                    //エラーテーブルに値を格納
                                    ErrCnt++;
                                    ErrorTableSet(iX, i, "貸", st[iX].Gyou[i].Kashi.Bumon, "枝番が未登録です。", MultiRow.DP_KASHI_CODEB);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// プロジェクトコードチェック
        /// </summary>
        /// <param name="iX">伝票配列データ添え字</param>
        /// <param name="st">伝票配列データ</param>
        //public void ChkProjectNEW(int iX, Entity.InputRecord[] st)
        //{
        //    //行ループ
        //    for (int i = 0; i < global.MAXGYOU; i++)
        //    {
        //        //取り消し行は対象外とする
        //        if (st[iX].Gyou[i].Torikeshi == "0")
        //        {
        //            //プロジェクトコードが登録されているとき
        //            if (st[iX].Gyou[i].ProjectCode.Trim() != string.Empty)
        //            {
        //                if (ChkProjectIndi(st[iX].Gyou[i].ProjectCode.Trim()) == false)
        //                {
        //                    //エラーテーブルに値を格納
        //                    ErrCnt++;
        //                    ErrorTableSet(iX, i, "プロジェクト", st[iX].Gyou[i].ProjectCode, "不正なプロジェクトコードです。", MultiRow.DP_CODEP);
        //                }
        //                else if (ChkProjectDateIndi(st[iX].Head.Year, st[iX].Head.Month, st[iX].Head.Day, company.Hosei) == false) //伝票日付とプロジェクト有効期限のチェック
        //                {
        //                    //エラーテーブルに値を格納
        //                    ErrCnt++;
        //                    ErrorTableSet(iX, i, "プロジェクト", st[iX].Gyou[i].ProjectCode, "有効期限外のプロジェクトです。", MultiRow.DP_CODEP);
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// サブプロジェクトコードチェック
        ///// </summary>
        ///// <param name="iX">伝票配列データ添え字</param>
        ///// <param name="st">伝票配列データ</param>
        //public void ChkSubProjectNEW(int iX, Entity.InputRecord[] st)
        //{
        //    //行ループ
        //    for (int i = 0; i < global.MAXGYOU; i++)
        //    {
        //        //取り消し行は対象外とする
        //        if (st[iX].Gyou[i].Torikeshi == "0")
        //        {
        //            if (ChkSubProjectIndi(st[iX].Gyou[i].SubProjectCode.Trim()) == false)
        //            {
        //                //エラーテーブルに値を格納
        //                ErrCnt++;
        //                ErrorTableSet(iX, i, "プロジェクト", st[iX].Gyou[i].SubProjectCode, "不正なサブプロジェクトコードです。", MultiRow.DP_CODESP);
        //            }
        //            else if (ChkSubProjectDateIndi(st[iX].Head.Year, st[iX].Head.Month, st[iX].Head.Day, company.Hosei) == false) //伝票日付とサブプロジェクト有効期限のチェック
        //            {
        //                //エラーテーブルに値を格納
        //                ErrCnt++;
        //                ErrorTableSet(iX, i, "プロジェクト", st[iX].Gyou[i].ProjectCode, "有効期限外のサブプロジェクトです。", MultiRow.DP_CODESP);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 消費税計算区分のコードチェック
        /// </summary>
        /// <param name="iX">伝票配列データ添え字</param>
        /// <param name="st">伝票配列データ</param>
        public void ChkOtherNEW(int iX, Entity.InputRecord[] st)
        {
            //行ループ
            for (int i = 0; i < st[iX].Gyou.Length; i++)
			{
                //取消行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    //消費税計算区分のチェック
                    //借方
                    if (ChkTaxMasIndi(st[iX].Gyou[i].Kari.TaxMas) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "借",st[iX].Gyou[i].Kari.TaxMas,"不正な税処理です。", MultiRow.DP_KARI_ZEI_S);
                    }
                
                    //貸方
                    if (ChkTaxMasIndi(st[iX].Gyou[i].Kashi.TaxMas) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "貸",st[iX].Gyou[i].Kashi.TaxMas,"不正な税処理です。", MultiRow.DP_KASHI_ZEI_S);
                    }
                }
			}
        }

        /// <summary>
        /// 税区分コードチェック
        /// </summary>
        /// <param name="iX">伝票配列データ添え字</param>
        /// <param name="st">伝票配列データ</param>
        public void ChkTaxKbnNEW(int iX, Entity.InputRecord[] st)
        {
            //行ループ
            for (int i = 0; i < st[iX].Gyou.Length; i++)
			{
                //取消行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    //借方
                    if (ChkTaxKbnIndi(st[iX].Gyou[i].Kari.TaxKbn) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "借",st[iX].Gyou[i].Kari.TaxKbn,"不正な税区分です", MultiRow.DP_KARI_ZEI);
                    }
                    //貸方
                    if (ChkTaxKbnIndi(st[iX].Gyou[i].Kashi.TaxKbn) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "貸",st[iX].Gyou[i].Kashi.TaxKbn,"不正な税区分です", MultiRow.DP_KASHI_ZEI);
                    }
                }
			}
        }

        /// <summary>
        /// 事業区分コードチェック
        /// </summary>
        /// <param name="iX">伝票配列データ添え字</param>
        /// <param name="st">伝票配列データ</param>
        public void ChkZigyoKbnNEW(int iX, Entity.InputRecord[] st)
        {
            //行ループ
            for (int i = 0; i < st[iX].Gyou.Length; i++)
            {
                //取消行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    //借方
                    if (ChkJigyo(st[iX].Gyou[i].Kari.JigyoKbn) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "借", st[iX].Gyou[i].Kari.JigyoKbn, "不正な事業区分です", MultiRow.DP_KARI_ZIGYO);
                    }
                    //貸方
                    if (ChkJigyo(st[iX].Gyou[i].Kashi.JigyoKbn) == false)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "貸", st[iX].Gyou[i].Kashi.JigyoKbn, "不正な事業区分です", MultiRow.DP_KASHI_ZIGYO);
                    }
                }
            }
        }

        ///--------------------------------------------------------
        /// <summary>
        ///     貸借不一致 及び　金額の不正チェック </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///--------------------------------------------------------
        public void ChkSumNEW(int iX, Entity.InputRecord[] st)
        {
            decimal sg;

            //同伝票の金額を加算
                        
            //----------->'複数枚チェック
            if (iX == 0)
            {
                Chkkin_IniTotal();
            }
            else if (st[iX].Head.FukusuChk == "0")
            {
                for (int i = 1; i <= global.pblFukumai + 1; i++)
                {
                    st[iX - i].Head.Kari_T = global.pblKari_T;
                    st[iX - i].Head.Kashi_T = global.pblKashi_T;
                }
                    
                //差額を求める
                sg = SumSagaku();
                if (sg != 0)
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX - 1, 0, "差額",string.Format("{0:#,##0}",sg),"貸借の金額に差額があります。", MultiRow.DP_SAGAKU_T);
                }

                Chkkin_IniTotal();
            }
            else
            {
                global.pblFukumai ++;
            }
          
            //--------------------------------------------------------------------------->

            //頁計初期化
            st[iX].KariTotal = 0;
            st[iX].KashiTotal = 0;
         
            //行毎ループ
            for (int i = 0; i < st[iX].Gyou.Length; i++)
            {
                //取消行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    //借方金額の不正チェック
                    if (ChkKinIndi(st[iX].Gyou[i].Kari.Kin) == false)
                    {
                        //借方金額のエラー
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "借",st[iX].Gyou[i].Kari.Kin,"不正な金額です。", MultiRow.DP_KARI_KIN);
                    }
            
                    //貸方金額の不正チェック
                    if (ChkKinIndi(st[iX].Gyou[i].Kashi.Kin) == false)
                    {
                        //借方金額のエラー
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "貸",st[iX].Gyou[i].Kashi.Kin,"不正な金額です。", MultiRow.DP_KASHI_KIN);
                    }
    
                    //借方合計加算
                    if (utility.NumericCheck(st[iX].Gyou[i].Kari.Kin))
                    {
                        //頁合計加算
                        st[iX].KariTotal += Decimal.Parse(st[iX].Gyou[i].Kari.Kin);
                        //伝票合計加算
                        global.pblKari_T += Decimal.Parse(st[iX].Gyou[i].Kari.Kin);
                    }
    
                    //貸方合計加算
                    if (utility.NumericCheck(st[iX].Gyou[i].Kashi.Kin))
                    {
                        //頁合計加算
                        st[iX].KashiTotal += Decimal.Parse(st[iX].Gyou[i].Kashi.Kin);
                        //伝票合計加算
                        global.pblKashi_T += Decimal.Parse(st[iX].Gyou[i].Kashi.Kin);
                    }
                }
            }
    
            //----------->'複数枚チェック
            //       最後の伝票？
            if ((iX + 1) == global.pblDenNum)
            {
                for (int i = 0; i <= global.pblFukumai; i++)
                {
                    st[iX - i].Head.Kari_T = global.pblKari_T;
                    st[iX - i].Head.Kashi_T = global.pblKashi_T;
                }
                
                sg = SumSagaku();
                if (sg != 0)
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX, 0, "差額",string.Format("{0:#,##0}",sg),"貸借の金額に差額があります。", MultiRow.DP_SAGAKU_T);
                }
            }
            //--------------------------------------------------------------------------->
        }

        ///----------------------------------------------------------------
        /// <summary>
        ///     相手科目未記入チェック </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///----------------------------------------------------------------
        public void ChkAiteNEW(int iX, Entity.InputRecord[] st)
        {
            //先頭レコードはフラグ初期化
            if (iX == 0) 
            {
                FLGClr();
            }
            //複数チェックなし
            else if (st[iX].Head.FukusuChk == "0")
            {
                //相手科目未記入エラー
                if (global.pblFlgKariKamoku == false && global.pblFlgKashiKamoku == true)
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX - 1, 1, "借方",string.Empty,"勘定科目が未記入です。", MultiRow.DP_KARI_CODE);
                }
            
                if (global.pblFlgKashiKamoku == false && global.pblFlgKariKamoku == true) 
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX - 1, 1, "貸方",string.Empty,"勘定科目が未記入です。", MultiRow.DP_KASHI_CODE);
                }
            
                FLGClr();
            }
    
            //勘定科目状態を調べる行毎ループ
            for (int i = 0; i < st[iX].Gyou.Length; i++)
            {
                //取消行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    //相手科目未記入チェック
                    if (st[iX].Gyou[i].Kari.Kamoku != string.Empty) global.pblFlgKariKamoku = true;
                    if (st[iX].Gyou[i].Kashi.Kamoku != string.Empty) global.pblFlgKashiKamoku = true;
                }
            }
    
            //伝票数まで達したら終了
            if  ((iX + 1) == global.pblDenNum)
            {
                //相手科目未記入エラー
                if (global.pblFlgKariKamoku == false && global.pblFlgKashiKamoku == true)
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX, 1, "借方",string.Empty,"勘定科目が未記入です。", MultiRow.DP_KARI_CODE);            
                }
        
                if (global.pblFlgKashiKamoku == false && global.pblFlgKariKamoku == true)
                {
                    //エラーテーブルに値を格納
                    ErrCnt++;
                    ErrorTableSet(iX, 1, "貸方",string.Empty,"勘定科目が未記入です。", MultiRow.DP_KASHI_CODE);            
                }
            }
        }

        ///----------------------------------------------------
        /// <summary>
        ///     摘要チェック </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///----------------------------------------------------
        public void ChkTekiyou(int iX, Entity.InputRecord[] st)
        {
            for (int i = 0; i < st[iX].Gyou.Length; i++)
            {
                //取消行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    // 借方、貸方いずれかに科目があるとき対象とする
                    if (st[iX].Gyou[i].Kari.Kamoku != string.Empty || st[iX].Gyou[i].Kashi.Kamoku != string.Empty)
                    {
                        //// 未記入はエラーとする
                        //if (st[iX].Gyou[i].Tekiyou == string.Empty)
                        //{
                        //    //エラーテーブルに値を格納
                        //    ErrCnt++;
                        //    ErrorTableSet(iX, i, "摘要", st[iX].Gyou[i].Tekiyou, "未記入です", MultiRow.DP_TEKIYOU);
                        //}

                        //摘要は１明細全角40、半角80がMAX
                        if (System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(st[iX].Gyou[i].Tekiyou) > 40)
                        {
                            //エラーテーブルに値を格納
                            ErrCnt++;
                            ErrorTableSet(iX, i, "摘要", st[iX].Gyou[i].Tekiyou, "入力文字数が超えています。", MultiRow.DP_TEKIYOU);
                        }
                    }
                }
            }
        }

        ///----------------------------------------------------------------
        /// <summary>
        ///     有効明細チェック </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///----------------------------------------------------------------
        public void ChkYukoMeisai(int iX, Entity.InputRecord[] st)
        {
            Boolean wrkRetValue = false;
    
            //行ループ
            for (int i = 0; i < st[iX].Gyou.Length; i++)
            {
                //取消行は対象外とする
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    //行全体が空欄のとき
                    if (st[iX].Gyou[i].Kari.Kamoku != string.Empty ||
                            st[iX].Gyou[i].Kari.Bumon != string.Empty ||
                            st[iX].Gyou[i].Kari.Hojo != string.Empty ||
                            st[iX].Gyou[i].Kari.Kin != string.Empty ||
                            st[iX].Gyou[i].Kari.TaxMas.Trim() != string.Empty ||
                            st[iX].Gyou[i].Kari.TaxKbn.Trim() != string.Empty ||
                            st[iX].Gyou[i].Kashi.Kamoku != string.Empty ||
                            st[iX].Gyou[i].Kashi.Bumon != string.Empty ||
                            st[iX].Gyou[i].Kashi.Hojo != string.Empty ||
                            st[iX].Gyou[i].Kashi.Kin != string.Empty ||
                            st[iX].Gyou[i].Kashi.TaxMas.Trim() != string.Empty ||
                            st[iX].Gyou[i].Kashi.TaxKbn.Trim() != string.Empty ||
                            st[iX].Gyou[i].Tekiyou.Trim() != string.Empty ||
                            st[iX].Gyou[i].ProjectCode.Trim() != string.Empty ||
                            st[iX].Gyou[i].SubProjectCode.Trim() != string.Empty)
                    {
                        wrkRetValue = true;
                        break;
                    }
                }
            }

            //有効明細なし
            if (wrkRetValue == false)
            {
                //エラーテーブルに値を格納
                ErrCnt++;
                ErrorTableSet(iX, 0, "借", "明細なし", "有効な明細がありません。", MultiRow.DP_KARI_CODE);
            }
        }

        /// <summary>
        /// 摘要のみチェック
        /// </summary>
        /// <param name="iX">伝票配列データ添え字</param>
        /// <param name="st">伝票配列データ</param>
        public void ChkTekiyouOnly(int iX, Entity.InputRecord[] st)
        {
            Boolean CheckFlg;
            Boolean wrkRetValue;
            int CntW = 0;

            //複数枚チェックがチェックされていない場合のみチェックを行う
            if (st[iX].Head.FukusuChk == "0")
            {
                CheckFlg = false;

                for (int i = 0; i < st[iX].Gyou.Length; i++)
			    {
                    if (st[iX].Gyou[i].Torikeshi == "0")
                    {
                        //取消行でない場合で、摘要の入力が１明細でもあれば摘要のみチェックを行う
                        if (st[iX].Gyou[i].Tekiyou.Trim() != string.Empty)
                        {
                            CheckFlg = true;
                            CntW = i;
                            break;
                        }
                    }
			    }
        
                if (CheckFlg == true)
                {
                    //摘要のみチェックを行う
                    wrkRetValue = false;

                    for (int i = 0; i < st[iX].Gyou.Length; i++)
			        {
                        if (st[iX].Gyou[i].Torikeshi == "0")
                        {
                            //取消行でない場合
                            if ((st[iX].Gyou[i].Kari.Kamoku != string.Empty) || (st[iX].Gyou[i].Kari.Kin != string.Empty) || 
                                (st[iX].Gyou[i].Kashi.Kamoku != string.Empty) || (st[iX].Gyou[i].Kashi.Kin != string.Empty))
                            {
                                //借方・貸方の科目または金額が入力されていればOK
                                wrkRetValue = true;
                                break;
                            }
                        }
			        }

                    if (wrkRetValue == false)
                    {
                        //摘要のみの場合
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, CntW, "摘要", st[iX].Gyou[CntW].Tekiyou, "勘定科目または金額が入力されていません。", MultiRow.DP_KARI_CODE);
                    }
                }
            }
        }

        ///-----------------------------------------------------
        /// <summary>
        ///     最初の行が摘要のみならエラーとします </summary>
        /// <param name="iX">
        ///     伝票配列インデックス</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///-----------------------------------------------------
        public void ChkFirstRowTekiyouOnly(int iX, Entity.InputRecord[] st)
        {
            // 最初の行が摘要のみならエラーとします　2012/10/07
            for (int i = 0; i < st[iX].Gyou.Length; i++)
            {
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    if (st[iX].Gyou[i].Kari.Kamoku == string.Empty &&
                        st[iX].Gyou[i].Kari.Bumon == string.Empty &&
                        st[iX].Gyou[i].Kari.Hojo == string.Empty &&
                        st[iX].Gyou[i].Kari.Kin == string.Empty &&
                        st[iX].Gyou[i].Kashi.Kamoku == string.Empty &&
                        st[iX].Gyou[i].Kashi.Bumon == string.Empty &&
                        st[iX].Gyou[i].Kashi.Hojo == string.Empty &&
                        st[iX].Gyou[i].Kashi.Kin == string.Empty &&
                        st[iX].Gyou[i].Tekiyou != string.Empty)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(iX, i, "摘要", st[iX].Gyou[i].Tekiyou, "最初の行が摘要のみになっています", MultiRow.DP_TEKIYOU);
                    }

                    break;
                }
            }

            // ２行続けて摘要のみは不可とします
            int sOn = 0;

            for (int i = 0; i < st[iX].Gyou.Length; i++)
            {
                if (st[iX].Gyou[i].Torikeshi == "0")
                {
                    if (st[iX].Gyou[i].Kari.Kamoku == string.Empty &&
                        st[iX].Gyou[i].Kari.Bumon == string.Empty &&
                        st[iX].Gyou[i].Kari.Hojo == string.Empty &&
                        st[iX].Gyou[i].Kari.Kin == string.Empty &&
                        st[iX].Gyou[i].Kashi.Kamoku == string.Empty &&
                        st[iX].Gyou[i].Kashi.Bumon == string.Empty &&
                        st[iX].Gyou[i].Kashi.Hojo == string.Empty &&
                        st[iX].Gyou[i].Kashi.Kin == string.Empty &&
                        st[iX].Gyou[i].Tekiyou != string.Empty)
                    {
                        if (sOn == 1)
                        {
                            //エラーテーブルに値を格納
                            ErrCnt++;
                            ErrorTableSet(iX, i, "摘要", st[iX].Gyou[i].Tekiyou, "２行続けて摘要のみは入力できません", MultiRow.DP_TEKIYOU);
                        }

                        sOn = 1;
                    }
                    else sOn = 0;

                }
            }
        }

        ///----------------------------------------------------
        /// <summary>
        ///     伝票番号重複チェック </summary>
        /// <param name="iX">
        ///     伝票配列データ添え字</param>
        /// <param name="st">
        ///     伝票配列データ</param>
        ///----------------------------------------------------
        public void ChkDenNumber(int iX, Entity.InputRecord[] st)
        {
            string dNum = st[iX].Head.DenNo;

            for (int i = 0; i < global.pblDenNum; i++)
            {
                // 自分以外で比較します
                if (iX != i)
                {
                    // ＮＧ、空白以外で他に同じ伝票№が存在したらエラー
                    if (dNum != "NG" && dNum != string.Empty && dNum == st[i].Head.DenNo)
                    {
                        //エラーテーブルに値を格納
                        ErrCnt++;
                        ErrorTableSet(i, 0, "№", st[i].Head.DenNo, "伝票№が重複しています。", MultiRow.DP_DENNO); 
                    }
                }
            }
        }

        /// <summary>
        /// エラー情報を配列に格納する
        /// </summary>
        /// <param name="iX">伝票枚数</param>
        /// <param name="sLine">行</param>
        /// <param name="sField">フィールド名</param>
        /// <param name="sData">エラーデータ</param>
        /// <param name="sNote">エラーメッセージ</param>
        /// <param name="sDpPos">MultiRowオブジェクト名</param>
        private void ErrorTableSet(int iX,int sLine,string sField,string sData,string sNote,string sDpPos)
        {
            if (ErrCnt > 1)
            {
                eTbl.CopyTo(eTbl = new Errtbl[ErrCnt], 0);
            }

            eTbl[ErrCnt - 1].Count = ErrCnt;
            eTbl[ErrCnt - 1].DenNo = iX;
            eTbl[ErrCnt - 1].LINE = sLine;
            eTbl[ErrCnt - 1].Field = sField;
            eTbl[ErrCnt - 1].Data = sData;
            eTbl[ErrCnt - 1].Notes = sNote;
            eTbl[ErrCnt - 1].DpPos = sDpPos;

        }
        
        ///--------------------------------------------------------
        /// <summary>
        ///     科目コードチェック : 勘定科目i10 2017/09/03 </summary>
        /// <param name="Kamoku">
        ///     科目コード</param>
        /// <returns>
        ///     true：OK、false:NG</returns>
        ///--------------------------------------------------------
        private Boolean ChkKamokuIndi(string Kamoku)
        {
            string KanjoCode = string.Empty;
 
            // 科目なしのときはOK
            if (Kamoku == string.Empty) return true;

            //////数字以外、3桁以上は×
            ////if ((utility.NumericCheck(Kamoku) == false) || (Kamoku.Length > 3)) return false;

            // 数字以外OK、3桁以上は×     2011/03/11
            if (Kamoku.Length > global.LEN_KAMOKU) return false;

            // 勘定科目取得
            if (utility.NumericCheck(Kamoku.Trim()))
            {
                KanjoCode = string.Format("{0:D10}", int.Parse(Kamoku.Trim()));
            }
            else
            {
                KanjoCode = Kamoku.Trim();
            }

            // 科目存在チェック
            // 勘定奉行データベースへ接続する
            string sc = SqlControl.obcConnectSting.get(global.pblDbName);

            //勘定奉行データベースへ接続する
            SqlControl.DataControl sCon = new SqlControl.DataControl(sc);

            string mySql = string.Empty;

            //////mySql += "select sUcd from wkskm01 ";
            //////mySql += "where tiIsTrk = 1 ";
            //////mySql += "and sUcd = '" + string.Format("{0,6}", Kamoku.Trim()) + "'";

            mySql += "SELECT AccountItemCode FROM tbAccountItem ";
            mySql += "WHERE (tbAccountItem.IsUse = 1) and ";
            mySql += "(tbAccountItem.AccountingPeriodID = " + global.pblAccPID + ") and ";
            mySql += "(AccountItemCode = '" + KanjoCode + "')";
            
            //データリーダーを取得する
            Boolean dRRows;
            SqlDataReader dR;
            dR = sCon.free_dsReader(mySql);
            dRRows = dR.HasRows;
            dR.Close();
            sCon.Close();

            return dRRows;
        }

        /// <summary>
        /// 補助コードチェック
        /// </summary>
        /// <param name="Hojo">補助コード</param>
        /// <param name="Kamoku">勘定科目コード</param>
        /// <returns></returns>
        private Boolean ChkHojoIndi(string Hojo,string Kamoku)
        {    
            Boolean wrkRetValue;

            string hojoCode = string.Empty;

            //科目と補助がなしのときはOK
            if (Kamoku == string.Empty && Hojo == string.Empty) return true;

            //勘定科目なし、補助ありはNG
            if (Kamoku == string.Empty && Hojo != string.Empty) return false;

            //空欄以外かつ数字以外、もしくは4桁以上は×
            //if ((Hojo != string.Empty && utility.NumericCheck(Hojo) == false) || Hojo.Length > 4) return false;

            //空欄以外かつ数字以外OK、4桁以上は×　2010/3/11
            if (Hojo.Length > global.LEN_HOJO) return false;

            //補助科目取得
            if (utility.NumericCheck(Hojo))
            {
                hojoCode = string.Format("{0:D10}", int.Parse(Hojo));
            }
            else
            {
                hojoCode = Hojo;
            }

            //勘定科目存在チェック
            // 勘定奉行データベース接続文字列を取得する 2017/09/04
            string sc = SqlControl.obcConnectSting.get(global.pblDbName);
            SqlControl.DataControl sCon = new SqlControl.DataControl(sc);
            string mySql = string.Empty;
            //////mySql += "select sNcd,sUcd,wkskm01.sNm,sHjoUcd,wkhjm01.sNm ";
            //////mySql += "from wkskm01 inner join wkhjm01 ";
            //////mySql += "on wkskm01.sNcd = wkhjm01.sSknNcd ";
            //////mySql += "where sHjoUcd <> '000000' and " + "sUcd = '" + string.Format("{0,6}",Kamoku) + "'";

            mySql += "select tbAccountItem.AccountItemID,tbAccountItem.AccountItemCode,";
            mySql += "tbAccountItem.AccountItemName,tbSubAccountItem.SubAccountItemCode,";
            mySql += "tbSubAccountItem.SubAccountItemName ";
            mySql += "from tbAccountItem inner join tbSubAccountItem ";
            mySql += "on tbAccountItem.AccountItemID = tbSubAccountItem.AccountItemID ";
            mySql += "where tbAccountItem.AccountingPeriodID = " + global.pblAccPID + " and ";
            mySql += "SubAccountItemCode <> '0000000000' and ";
            mySql += "tbAccountItem.AccountItemCode = '" + Kamoku + "'";

            //データリーダーを取得する
            SqlDataReader dR;
            dR = sCon.free_dsReader(mySql);

            //補助記入が無し、勘定科目の補助設定が無い場合はＯＫ
            if ((Hojo == string.Empty && dR.HasRows == false)) 
            {
                wrkRetValue = true;
            }
            //補助記入が有り、勘定科目の補助設定が無い場合はNG
            else if ((Hojo != string.Empty && dR.HasRows == false)) 
            {
                wrkRetValue = false;
            }
            //補助記入が無く、勘定科目の補助設定がある場合はNG
            else if ((Hojo == string.Empty && dR.HasRows == true))  
            {
                wrkRetValue = false;
            }
            else
            {
                //補助の記入があり、勘定科目の補助設定が有る場合その他を含めた補助科目データリーダーを再取得する
                mySql = string.Empty;
                //////mySql += "select sNcd,sUcd,wkskm01.sNm,sHjoUcd,wkhjm01.sNm ";
                //////mySql += "from wkskm01 inner join wkhjm01 ";
                //////mySql += "on wkskm01.sNcd = wkhjm01.sSknNcd ";
                //////mySql += "where sUcd = '" + string.Format("{0,6}", Kamoku) + "'";

                mySql += "select tbAccountItem.AccountItemID,tbAccountItem.AccountItemCode,";
                mySql += "tbAccountItem.AccountItemName,tbSubAccountItem.SubAccountItemCode,";
                mySql += "tbSubAccountItem.SubAccountItemName ";
                mySql += "from tbAccountItem inner join tbSubAccountItem ";
                mySql += "on tbAccountItem.AccountItemID = tbSubAccountItem.AccountItemID ";
                mySql += "where tbAccountItem.AccountingPeriodID = " + global.pblAccPID + " and ";
                mySql += "tbAccountItem.AccountItemCode = '" + Kamoku + "'";

                //データリーダーを取得する
                dR.Close();
                dR = sCon.free_dsReader(mySql);

                //補助科目リストループ
                wrkRetValue = false;
                while (dR.Read())
	            {
                    //補助コードが該当すればＯＫ
                    if (dR["SubAccountItemCode"].ToString().Trim() == hojoCode)
                    {
                        wrkRetValue = true;
                        break;	         
                    }
	            }
            }

            dR.Close();
            sCon.Close();

            return wrkRetValue;
        }

        ///---------------------------------------------------------
        /// <summary>
        ///     部門コードチェック : 勘定奉行i10 2017/09/03 </summary>
        /// <param name="Bumon">
        ///     部門コード</param>
        /// <returns>
        ///     ok:true,NG:false</returns>
        ///---------------------------------------------------------
        private Boolean ChkBumonIndi(string Bumon)
        {
            Boolean wrkRetValue;
            string CodeB;

            //部門なしのときはOK
            if (Bumon == string.Empty) return true;
        
            //部門登録が無しで、部門記入がある時NG
            if (global.pblBumonFlg == false && Bumon != string.Empty) return false;

            ////////数字以外、4桁以上は×
            //////if ((utility.NumericCheck(Bumon) == false || Bumon.Length > 4)) return false;

            //数字以外はOK、5桁以上は×    2011/03/11
            if (Bumon.Length > global.LEN_BUMON) return false;

            //部門コード編集
            if (utility.NumericCheck(Bumon))
            {
                //////if (Bumon != "0")
                //////{
                //////    CodeB = string.Format("{0,6}", int.Parse(Bumon));
                //////}
                //////else
                //////{
                //////    CodeB = string.Format("{0:000000}", int.Parse(Bumon));
                //////}

                CodeB = string.Format("{0:D15}", int.Parse(Bumon));
            }
            else
            {
                CodeB = Bumon;
            }

            // 接続文字列取得 2017/09/03
            string sc = SqlControl.obcConnectSting.get(global.pblDbName);

            // 勘定奉行データベースへ接続する
            //string sc = utility.GetDBConnect(global.pblDbName);
            SqlControl.DataControl sCon = new SqlControl.DataControl(sc);

            string mySql = string.Empty;
            //////mySql += "SELECT sUcd,sNm from wkbnm01 ";
            //////mySql += "where sUcd = '" + CodeB + "'";
            
            mySql += "select DepartmentID, DepartmentCode, DepartmentName from tbDepartment ";
            mySql += "where tbDepartment.DepartmentCode = '" + CodeB + "'";

            //データリーダーを取得する
            SqlDataReader dR;
            dR = sCon.free_dsReader(mySql);
            wrkRetValue = dR.HasRows;
            dR.Close();
            sCon.Close();

            return wrkRetValue;
        }

        /// <summary>
        /// 消費税計算区分のコードチェック
        /// </summary>
        /// <param name="TaxMas">消費税計算区分</param>
        /// <returns>ok:true,NG:false</returns>
        private Boolean ChkTaxMasIndi(string TaxMas)
        {
            //未記入、2か1か0ならＯＫ
            if (TaxMas == string.Empty || TaxMas == "2" || TaxMas == "1" || TaxMas == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 事業区分のコードチェック
        /// </summary>
        /// <param name="TaxMas">事業区分</param>
        /// <returns>ok:true,NG:false</returns>
        private Boolean ChkJigyo(string sJigyo)
        {
            //未記入、2か1か0ならＯＫ
            if (sJigyo == string.Empty || sJigyo == "0005" || sJigyo == "0004" || sJigyo == "0003" || 
                sJigyo == "0002" || sJigyo == "0001" || sJigyo == "5" || sJigyo == "4" || 
                sJigyo == "3" || sJigyo == "2" || sJigyo == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        ///----------------------------------------------------
        /// <summary>
        ///     税区分コードチェック </summary>
        /// <param name="TaxKbn">
        ///     税区分コード</param>
        /// <returns>
        ///     ok:true,NG:false</returns>
        ///----------------------------------------------------
        private Boolean ChkTaxKbnIndi(string TaxKbn)
        {
            Boolean wrkRetValue;

            //税区分なしのときはOK
            if (TaxKbn == string.Empty) return true;
        
            //数字以外、4桁以上は×
            if ( utility.NumericCheck(TaxKbn) == false || TaxKbn.Length > 4) return false;
    
            //税区分存在チェック

            //勘定奉行データベースへ接続する
            string sc = utility.GetDBConnect(global.pblDbName);
            SqlControl.DataControl sCon = new SqlControl.DataControl(sc);
            string mySql = string.Empty;

            ////mySql += "SELECT tiZeiCd FROM wktax01 ";
            ////mySql += "where tiZeiCd = '" + TaxKbn + "'";

            mySql += "select TaxDivisionCode,TaxDivisionName from tbTaxDivision ";
            mySql += "WHERE AccountingPeriodID = " + global.pblAccPID + " and ";
            mySql += "TaxDivisionCode = '" + string.Format("{0:D4}",int.Parse(TaxKbn)) + "'";

            //データリーダーを取得する
            SqlDataReader dR;
            dR = sCon.free_dsReader(mySql);
            wrkRetValue = dR.HasRows;
            dR.Close();
            sCon.Close();

            return wrkRetValue;
        }

        /// <summary>
        /// 金額チェック
        /// </summary>
        /// <param name="Kingaku">金額</param>
        /// <returns>ok:true,NG:false</returns>
        public static Boolean ChkKinIndi(string Kingaku)
        {  
            //金額未記入のときはOK
            if (Kingaku == string.Empty) return true;
        
            //数字以外、11桁以上は×
            if (utility.NumericCheck(Kingaku) == false || Kingaku.Length > 10) return false;
    
            //金額が０のときはNG
            if (Kingaku == "0") return false;
    
            //最後が"-"はNG
            if (Kingaku.Substring(Kingaku.Length - 1,1) == "-") return false;

            return true;
        }

        //伝票合計金額初期化
        private void Chkkin_IniTotal()
        {
            global.pblKari_T = 0;
            global.pblKashi_T = 0;
            global.pblFukumai = 0;
        }

        /// <summary>
        /// 貸借差額の算出
        /// </summary>
        /// <returns>差額の絶対値</returns>
        private decimal SumSagaku()
        {
            //貸借差額計算
            decimal Sagaku = System.Math.Abs(global.pblKari_T - global.pblKashi_T);
            return Sagaku;
        }
    
        /// <summary>
        /// 借方貸方科目ステータス初期化
        /// </summary>
        /// <returns></returns>
        private void FLGClr()
        {
            global.pblFlgKariKamoku = false;
            global.pblFlgKashiKamoku = false;
        }
    }
}

