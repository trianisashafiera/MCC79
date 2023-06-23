﻿using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly BookingDbContext _context;
        public RoomRepository(BookingDbContext context)
        {
            _context = context;
        }

        public ICollection<Room> GetAll()
        {
            return _context.Set<Room>().ToList();
        }

        public Room? GetByGuid(Guid guid)
        {
            return _context.Set<Room>().Find(guid);
        }

        public Room Create(Room room)
        {
            try
            {
                _context.Set<Room>().Add(room);
                _context.SaveChanges();
                return room;
            }
            catch
            {
                return new Room();
            }
        }

        public bool Update(Room room)
        {
            try
            {
                _context.Set<Room>().Update(room);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Room room)
        {
            try
            {
                var room = GetByGuid(guid);
                if(room is null)
                {
                    return false;
                }
                _context.Set<Room>().Remove(room);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
