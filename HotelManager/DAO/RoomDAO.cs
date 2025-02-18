﻿using HotelManager.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.DAO
{
    public class RoomDAO
    {
        private static RoomDAO instance;

        #region Method

        internal DataTable LoadFullRoom()
        {
            return DataProvider.Instance.ExecuteQuery("USP_LoadFullRoom");
        }

        internal bool InsertRoom(Room roomNow)
        {
            return InsertRoom(roomNow.Name, roomNow.IdRoomType, roomNow.IdStatusRoom);
        }

        internal bool InsertRoom(string roomName, int idRoomType, int idStatusRoom)
        {
            string query = "USP_InsertRoom @nameRoom , @idRoomType , @idStatusRoom";
            return DataProvider.Instance.ExecuteNoneQuery(query, new object[] { roomName, idRoomType, idStatusRoom }) > 0;
        }

        public bool CheckRoomNameExists(string nameRoom, int idRoomType)
        {
            string query = "USP_CheckRoomNameExists @nameRoom , @idRoomType";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { nameRoom, idRoomType });
            return result.Rows.Count > 0 && (int)result.Rows[0]["RoomExists"] == 1;
        }

        internal bool UpdateCustomer(Room roomNow)
        {
            string query = "USP_UpdateRoom @id , @nameRoom , @idRoomType , @idStatusRoom";
            return DataProvider.Instance.ExecuteNoneQuery(query, new object[] { roomNow.Id, roomNow.Name, roomNow.IdRoomType, roomNow.IdStatusRoom }) > 0;
        }

        internal DataTable Search(string text, int id)
        {
            string query = "USP_SearchRoom @string , @id";
            return DataProvider.Instance.ExecuteQuery(query, new object[] { text, id });
        }

        public List<Room> LoadEmptyRoom(int idRoomType)
        {
            List<Room> rooms = new List<Room>();
            string query = "USP_LoadEmptyRoom @idRoomType";
            DataTable data = DataProvider.Instance.ExecuteQuery(query, new object[] { idRoomType });
            foreach (DataRow item in data.Rows)
            {
                Room room = new Room(item);
                rooms.Add(room);
            }
            return rooms;
        }

        public List<Room> LoadListFullRoom()
        {
            string query = "USP_LoadListFullRoom @getToday";
            List<Room> rooms = new List<Room>();
            DataTable data = DataProvider.Instance.ExecuteQuery(query, new object[] { DateTime.Now.Date });
            foreach (DataRow item in data.Rows)
            {
                Room room = new Room(item);
                rooms.Add(room);
            }
            return rooms;
        }

        public int GetPeoples(int idBill)
        {
            string query = "USP_GetPeoples @idBill";
            return (int)DataProvider.Instance.ExecuteScalar(query, new object[] { idBill }) + 1;
        }

        public int GetIdRoomFromReceiveRoom(int idReceiveRoom)
        {
            string query = "USP_GetIDRoomFromReceiveRoom @idReceiveRoom";
            return (int)DataProvider.Instance.ExecuteScalar(query, new object[] { idReceiveRoom });
        }

        public bool UpdateStatusRoom(int idRoom, int status)
        {
            string query = "USP_UpdateStatusRoom @idRoom , @status";
            return DataProvider.Instance.ExecuteNoneQuery(query, new object[] { idRoom, status }) > 0;
        }

        #endregion

        public bool DeleteRoom(int id)
        {
            string query = "EXEC USP_DeleteRoom @id";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { id });

            if (result.Rows.Count > 0)
            {
                int deleteResult = Convert.ToInt32(result.Rows[0]["Result"]);
                if (deleteResult == -1)
                {
                    return false; // Room in use
                }
                else if (deleteResult == -2)
                {
                    throw new Exception("The room is referenced by another record and cannot be deleted.");
                }
                else if (deleteResult == 1)
                {
                    return true;
                }
                else if (deleteResult == 0)
                {
                    return false; // Room not found
                }
            }
            return false;
        }

        public static RoomDAO Instance
        {
            get { if (instance == null) instance = new RoomDAO(); return instance; }
            private set => instance = value;
        }

        private RoomDAO() { }
    }
}
