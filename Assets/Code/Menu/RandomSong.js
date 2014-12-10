#pragma strict
var songs : AudioClip[];

function Update() {
	if (!audio.isPlaying)
	playRandomMusic();
}

function playRandomMusic() {
	audio.clip = songs[Random.Range(0,songs.length)];
	audio.Play();
}