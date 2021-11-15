const meetingRoomId = document.getElementById("MeetingRoomId").value;
const meetingsUiList = document.getElementById('meetings');
const meetingTimeHeader = document.getElementById('meeting-time');
const attendingCompanyHeader = document.getElementById('attending-company');
const attendoParagraph = document.getElementById('attendees');
let meetings = [];

window.setInterval(async function () {
    setTime();
    await getMeetings();
    checkMeetings();
    appendToUiList();
}, 3000);

function setTime() {
    const today = new Date();
    const currentTime = `${today.getHours()}:${today.getMinutes() < 10 ? '0' : ''}${today.getMinutes()}`;
    const timeSpan = document.getElementById('time');
    timeSpan.innerHTML = currentTime;
}

async function getMeetings() {
    const response = await fetch(`/Display/GetMeetings?meetingRoomId=${meetingRoomId}`);
    const data = await response.json();
    data.forEach(x => meetings.push(x));
}

function checkMeetings() {
    if (meetings.length === 0) {
        toggleAvailableState();
    }
    const currentDate = new Date();
    const firstMeeting = meetings[0];
    const firstMeetingDate = new Date(firstMeeting.startDateTime);
    if (firstMeetingDate.getTime() <= currentDate.getTime()) { // Meeting busy
        toggleBusyState(meetings[0]);
    }
    else if (getDiffInMinutes(currentDate, firstMeetingDate) < 15) { // Meeting starts within 15 minutes
        toggleAlmostState(meetings[0]);
    }
    else { // No meeting within 15 minutes
        toggleAvailableState();
    }
}

function appendToUiList() {
    meetingsUiList.innerHTML = '';
    meetings.forEach(meeting => {
        let li = document.createElement('li');
        li.appendChild(document.createTextNode(`${meeting.attendingCompany} ${meeting.startShortTimeString} - ${meeting.endShortTimeString} `));
        li.setAttribute('id', meeting.id);
        meetingsUiList.appendChild(li);
    });
    meetings = [];
}

function toggleBusyState(meeting) {
    meetingTimeHeader.innerHTML = `${meeting.startShortTimeString} - ${meeting.endShortTimeString} `;
    attendingCompanyHeader.innerHTML = meeting.attendingCompany;
    attendoParagraph.innerHTML = appendAttendees(meeting);
    document.body.classList.add('busy').remove('available').remove('almost');
    meetings.shift();
}

function toggleAlmostState(meeting) {
    meetingTimeHeader.innerHTML = `${meeting.startShortTimeString} - ${meeting.endShortTimeString} `;
    attendingCompanyHeader.innerHTML = meeting.attendingCompany;
    document.body.classList.add('almost').remove('available').remove('busy');
    attendoParagraph.innerHTML = '';
}

function toggleAvailableState() {
    meetingTimeHeader.innerHTML = '';
    attendingCompanyHeader.innerHTML = '';
    document.body.classList.add('available').remove('almost').remove('busy');
    attendoParagraph.innerHTML = '';
}

function appendAttendees(meeting) {
    if (meeting.attendees.length > 0) {
        const attendees = meeting.attendees.join(' ');
        return attendees;
    }
    return;
}

function getDiffInMinutes(date1, date2) {
    var diff = (date2.getTime() - date1.getTime()) / 1000;
    diff /= 60;
    return Math.abs(Math.round(diff));
}