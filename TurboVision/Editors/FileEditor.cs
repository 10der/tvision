#region Using directives

using System;
#if FRAMEWORK20
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Text;
using TurboVision.Objects;
using TurboVision.StdDlg;
using TurboVision.Views;

#endregion

namespace TurboVision.Editors
{
    public class FileEditor : Editor
    {

        public const int cmSave        = 32;
        public const int cmSaveAs      = 33;

        public string FileName = "";

        public FileEditor( Rect Bounds, ScrollBar AHScrollBar, ScrollBar AVScrollBar, Indicator AIndicator, string AFileName)
            :base( Bounds, AHScrollBar, AVScrollBar, AIndicator, 0)
        {
            if (AFileName != "")
            {
                FileName = System.IO.Path.GetFullPath(AFileName);
                if (IsValid)
                    IsValid = LoadFile();
            }
        }

        public override bool SetBufSize(int NewSize)
        {
            long N;
            char[] P;
            bool SetBufSize = false;
            if (NewSize == 0)
                NewSize = 0x1000;
            else
                NewSize = ((NewSize + 0x0FFF) & 0xFFFF000);
            if (NewSize != BufSize)
            {
                if (NewSize > BufSize)
                {
                    try
                    {
                        P = new char[NewSize];
                        Array.Copy( buffer, 0, P, 0, BufSize);
                        buffer = P;
                    }
                    catch
                    {
                        return SetBufSize;
                    }
                }
                N = BufLen - CurPtr + DelCount;
                Array.Copy(buffer, BufSize - N, buffer, NewSize - N, N);
                if (NewSize < BufSize)
                {
                    P = new char[NewSize];
                    if (P == null)
                        NewSize = BufSize;
                    else
                    {
                        Array.Copy(buffer, 0, P, 0, NewSize);
                        buffer = P;
                    }
                }
                BufSize = NewSize;
                GapLen = BufSize - BufLen;
            }
            return true;
        }

        public bool LoadFile()
        {
            int Length = 0;
            bool LoadFile = true;
            int FSize = 0;

            if (FileName == "")
                return LoadFile;
            LoadFile = false;
            Length = 0;

            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(
                    FileName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
                System.IO.StreamReader sr = new System.IO.StreamReader(fs, Encoding.GetEncoding(866));
                FSize = (int)fs.Length;
                if (!SetBufSize(FSize))
                    editorDialog(edOutOfMemory, null);
                else
                {
                    try
                    {
                        char[] tempBuf = new char[FSize];
                        sr.Read(tempBuf, 0, (int)FSize);
                        Array.Copy(tempBuf, 0, buffer, BufSize - FSize, FSize);
                        LoadFile = true;
                        Length = FSize;
                    }
                    catch (System.IO.IOException Ex)
                    {
                        //editorDialog(edReadError, FileName);
                        ErrorBox.Show("Ошибка ввода-вывода", Ex.Message, Ex.StackTrace);
                    }
                }
                fs.Close();
            }
            catch
            {
                editorDialog(edReadError, FileName);
            }
            SetBufLen(Length);
            return LoadFile;
        }

        public override void InitBuffer()
        {
            buffer = new char[0x1000];
        }

        public bool Save()
        {
            if (FileName == "")
                return /*SaveAs()*/false;
            else
                return SaveFile();
        }

        public bool SaveFile()
        {
            try
            {
                if ((EditorFlags & efBackupFiles) != 0)
                {
                    try
                    {
                        if (System.IO.File.Exists(System.IO.Path.ChangeExtension(FileName, "bak")))
                            System.IO.File.Delete(System.IO.Path.ChangeExtension(FileName, "bak"));
                        System.IO.File.Copy(FileName, System.IO.Path.ChangeExtension( FileName, "bak"));
                    }
                    catch (System.IO.IOException Ex)
                    {
                        MsgBox.MessageBox("Не могу создать резервную копию.\x000D\x0003" + Ex.Message);
                        return false;
                    }
                }
                System.IO.StreamWriter tw = new System.IO.StreamWriter(FileName,false, System.Text.Encoding.GetEncoding(866));
                char[] bf = new char[CurPtr];
                Array.Copy(buffer, 0, bf, 0, CurPtr);
                tw.Write(bf);
                bf = new char[BufLen - CurPtr];
                Array.Copy(buffer, CurPtr + GapLen, bf, 0, BufLen - CurPtr);
                tw.Write(bf);
                tw.Close();
                return true;
            }
            catch (System.IO.IOException Ex)
            {
                MsgBox.MessageBox("\x0003Не могу сохранить файл.\x000D\x0003" + Ex.Message);
                return false;
            }
        }

        public override void HandleEvent(ref Event Event)
        {
            base.HandleEvent( ref Event);
            switch (Event.What)
            {
                case Event.evCommand :
                    switch( Event.Command)
                    {
                        case cmSave:
                            Save();
                            break;
//                        case cmSaveAs :
//                            SaveAs();
//                            break;
                        default :
                            return;
                    }
                    break;
                default:
                    return;
            }
            ClearEvent(ref Event);
        }


        public override void Done()
        {
            if( !IsClipBoard)
                Save();
            base.Done();
        }

        public static int EditorFlags = efBackupFiles + efPromptOnReplace;
    }
}
