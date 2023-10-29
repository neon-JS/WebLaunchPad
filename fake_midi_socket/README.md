# fake_midi_socket

This program opens up a unix socket listener and listens to incoming data. 
It forwards the incoming data to a midi device that is currently connected to
the pc. This can be useful when working on a system that doesn't create a file
handle for every midi device.

## Warning
**This is not meant for production!** I use this tool while working on this repo
on a mac (where file handles aren't created automatically). It should "just work"
and is therefore not hardened, really tested or comfortable to configure.
**Handle with care!**

## Usage
./fake_midi_socket /tmp/midi0

## Exit codes
0. Ok
1. No socket address (to listen on) given
2. No midi device found
3. Error while transmitting midi data
4. Unix socket error