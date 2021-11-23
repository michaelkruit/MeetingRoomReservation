// Selected meeting room Id from DOM
const meetingRoomId = document.getElementById("MeetingRoomId").value;
// Meetings list from DOM
const meetingsUiList = document.getElementById('meetings');
// Meeting time header from DOM
const meetingTimeHeader = document.getElementById('meeting-time');
// Meeeting attending company header from DOM
const attendingCompanyHeader = document.getElementById('attending-company');
// Attendees paragaph from DOM
const attendeesParagraph = document.getElementById('attendees');
// Array that will contain the fetched meetings
let meetings = [];

// Execute following functions every 3 seconds
window.setInterval(async function () {
    // Set current time
    setTime();
    // Get meeting asynchronis
    await getMeetings();
    // Check if there are meetings available 
    checkMeetings();
    // Append meetings to ul in DOM
    appendToUiList();
}, 3000);

// Set current time 
function setTime() {
    // Get current date
    const today = new Date();
    // Create current time string
    const currentTime = `${today.getHours()}:${today.getMinutes() < 10 ? '0' : ''}${today.getMinutes()}`;
    // Get DOM element that contains current time
    const timeSpan = document.getElementById('time');
    // Update element with current time
    timeSpan.innerHTML = currentTime;
}

// Get meetings 
async function getMeetings() {
    // Fetch meetings from server
    const response = await fetch(`/Display/GetMeetings?meetingRoomId=${meetingRoomId}`);
    // Get json response 
    const data = await response.json();
    // Push meetings to meetings array
    data.forEach(x => meetings.push(x));
}

// Check meetings and set state 
function checkMeetings() {
    // When no meetings where fetched
    if (meetings.length === 0) {
        toggleAvailableState();
    }
    // Get current date
    const currentDate = new Date();
    // Get first meeting from the array
    const firstMeeting = meetings[0];
    // Create date object based on the start datetime of the first meeting
    const firstMeetingDate = new Date(firstMeeting.startDateTime);
    // Meeting busy
    if (firstMeetingDate.getTime() <= currentDate.getTime()) {
        toggleBusyState(meetings[0]);
    }
    // Meeting starts within 15 minutes
    else if (getDiffInMinutes(currentDate, firstMeetingDate) < 15) {
        toggleAlmostState(meetings[0]);
    }
    // No meeting within 15 minutes
    else {
        toggleAvailableState();
    }
}

// Add meetings to UI list
function appendToUiList() {
    // Empty inner HTML of list
    meetingsUiList.innerHTML = '';
    // Loop through meetings array
    meetings.forEach(meeting => {
        // Create new li DOM item
        let li = document.createElement('li');
        // Add meeting information to li item
        li.appendChild(document.createTextNode(`${meeting.attendingCompany} ${meeting.startShortTimeString} - ${meeting.endShortTimeString} `));
        // Append li element to ul
        meetingsUiList.appendChild(li);
    });
    // Empty meetings array
    meetings = [];
}

// Toggle busy state
function toggleBusyState(meeting) {
    // Change meeting time header with current meeting times
    meetingTimeHeader.innerHTML = `${meeting.startShortTimeString} - ${meeting.endShortTimeString} `;
    // Change meeting attending company with current meeting company
    attendingCompanyHeader.innerHTML = meeting.attendingCompany;
    // Set attendees in pargraph
    attendeesParagraph.innerHTML = appendAttendees(meeting);
    // Change class of body to busy and remove other classes
    document.body.classList.add('busy');
    document.body.classList.remove('available');
    document.body.classList.remove('almost');
    // Remove meeting from meetings array
    meetings.shift();
}

// Toggle almost busy state
function toggleAlmostState(meeting) {
    // Change meeting time header with coming meeting times
    meetingTimeHeader.innerHTML = `${meeting.startShortTimeString} - ${meeting.endShortTimeString} `;
    // Change meeting attending company with comming meeting company
    attendingCompanyHeader.innerHTML = meeting.attendingCompany;
    // Set attendees in pargraph from coming meeting
    attendeesParagraph.innerHTML = appendAttendees(meeting);
    // Change class of body to almost and remove other classes
    document.body.classList.add('almost');
    document.body.classList.remove('available');
    document.body.classList.remove('busy');
    // Remove meeting from meetings array
    meetings.shift();
}

// Toggle availabe state
function toggleAvailableState() {
    // Clear meeting time header 
    meetingTimeHeader.innerHTML = '';
    // Clear attending company header
    attendingCompanyHeader.innerHTML = '';
    // Change class of body to available and remove other classes
    document.body.classList.add('available');
    document.body.classList.remove('almost');
    document.body.classList.remove('busy');
    // Remove attendees from pargraph
    attendeesParagraph.innerHTML = '';
}

// Append attendees to container div
function appendAttendees(meeting) {
    // Check if meetings contains attendees
    if (meeting.attendees.length > 0) {
        // Join attendees array to one string
        const attendees = meeting.attendees.join(' ');
        return attendees;
    }
    return;
}

// Get difference in minutes from two different date times
function getDiffInMinutes(date1, date2) {
    var diff = (date2.getTime() - date1.getTime()) / 1000;
    diff /= 60;
    return Math.abs(Math.round(diff));
}