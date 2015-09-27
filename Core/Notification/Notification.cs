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

using Newtonsoft.Json.Linq;

namespace FOG.Core
{
    /// <summary>
    ///     Store neccesary notification information
    /// </summary>
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string SubjectID { get; set; }
        public bool OnGoing { get; set; }

        public Notification()
        {
            Title = "";
            Message = "";
            SubjectID = "";
            OnGoing = false;
        }

        public Notification(string title, string message, string subjectID = "", bool onGoing = false)
        {
            Title = title;
            Message = message;
            SubjectID = subjectID;
            OnGoing = onGoing;
        }

        public JObject GetJson()
        {
            dynamic json = new JObject();
            json.title = Title;
            json.message = Message;
            json.subjectID = SubjectID;
            json.onGoing = OnGoing;
            return json;
        }
    }
}