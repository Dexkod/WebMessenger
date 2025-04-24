"use strict";

let mediaStreamConstraints = {
    video: true
};

let offerOptions = {
    offerToReceiveVideo: 1
};

export function callVideo() {
    mediaStreamConstraints = {
        video: true,
        audio: true
    };

    const offerOptions = {
        offerToReceiveVideo: 1,
        offerToReceiveAudio: 1
    };
}

const servers = {
    iceServers: [
        {
            urls: "turn:coturn.myserver.com:3478",
            username: "username",
            credential: "password"
        }
    ]
}

let dotNet;
let localStream;
let remoteStream;
let peerConnection;

let isOffering;
let isOffered;

export function initialize(dotNetRef) {
    dotNet = dotNetRef;
}
export async function startLocalStream() {
    localStream = await navigator.mediaDevices.getUserMedia(mediaStreamConstraints);
    return localStream;
}

function createPeerConnection() {
    if (peerConnection != null) return;
    peerConnection = "hello";
    peerConnection = new RTCPeerConnection(servers);
    peerConnection.addEventListener("icecandidate", handleConnection);
    peerConnection.addEventListener("iceconnectionstatechange", handleConnectionChange);
    peerConnection.addEventListener("addstream", gotRemoteMediaStream);

    peerConnection.addStream(localStream);
}

export async function callAction() {
    if (isOffered) return Promise.resolve();

    isOffering = true;
    createPeerConnection();

    let offerDescription = await peerConnection.createOffer(offerOptions);
    await peerConnection.setLocalDescription(offerDescription);
    return JSON.stringify(offerDescription);
}

export async function processAnswer(descriptionText) {
    let description = JSON.parse(descriptionText);
    await peerConnection.setRemoteDescription(description);
}


export async function processOffer(descriptionText) {
    if (isOffering) return;

    createPeerConnection();
    let description = JSON.parse(descriptionText);
    await peerConnection.setRemoteDescription(description);

    let answer = await peerConnection.createAnswer();
    await peerConnection.setLocalDescription(answer);

    dotNet.invokeMethodAsync("SendAnswer", JSON.stringify(answer));
}

export async function processCandidate(candidateText) {
    let candidate = JSON.parse(candidateText);
    await peerConnection.addIceCandidate(candidate);
}

export function hangupAction() {
    peerConnection.removeEventListener("icecandidate", handleConnection);
    peerConnection.removeEventListener("iceconnectionstatechange", handleConnectionChange);
    peerConnection.removeEventListener("addstream", gotRemoteMediaStream);
    peerConnection.close();
    peerConnection = null;

    localStream.getTracks().forEach(track => track.stop());
    localStream = null;

    remoteStream.getTracks().forEach(track => track.stop());
    remoteStream = null;

    isOffering = false;
    isOffered = false;
}

// Handles remote MediaStream success by handing the stream to the blazor component.
async function gotRemoteMediaStream(event) {
    const mediaStream = event.stream;
/*    console.log(mediaStream);*/
    remoteStream = mediaStream;
    await dotNet.invokeMethodAsync("SetRemoteStream");
/*    console.log("Remote peer connection received remote stream.");*/
}
export function getRemoteStream() {
    return remoteStream;
}

// Sends candidates to peer through signaling.
async function handleConnection(event) {
    const iceCandidate = event.candidate;

    if (iceCandidate) {
        await dotNet.invokeMethodAsync("SendCandidate", JSON.stringify(iceCandidate));

/*        console.log(`peerConnection ICE candidate:${event.candidate.candidate}.`);*/
    }
}

// Logs changes to the connection state.
function handleConnectionChange(event) {
    const peerConnection = event.target;
    //console.log("ICE state change event: ", event);
    //console.log(`peerConnection ICE state: ${peerConnection.iceConnectionState}.`);
}


