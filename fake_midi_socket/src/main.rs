mod errors;

use errors::TransmissionError;
use midir::{MidiOutput, MidiOutputConnection};
use std::env;
use std::fmt::Display;
use std::io::Read;
use std::os::unix::net::{UnixListener, UnixStream};
use std::process::exit;

const MAX_SYSEX_MESSAGE_LENGTH: usize = 255;

const EXIT_CODE_NO_SOCKET_ADDRESS: i32 = 1;
const EXIT_CODE_NO_MIDI_DEVICE_FOUND: i32 = 2;
const EXIT_CODE_MIDI_TRANSMISSION_ERROR: i32 = 3;
const EXIT_CODE_UNIX_SOCKET_ERROR: i32 = 4;

fn main() {
    print_debug_message("Debug messages enabled");

    let socket_address = match get_socket_address() {
        Some(socket_address) => socket_address,
        None => exit_with_error(
            "No socket address given. Usage: fake_midi_socket <SOCKET_ADDRESS>",
            EXIT_CODE_NO_SOCKET_ADDRESS,
            None,
        ),
    };

    print_debug_message(format!("Socket address: '{}'", socket_address).as_str());
    print_debug_message("Setting up midi connection...");

    let mut midi_connection = match get_midi_output_connection() {
        Some(midi_connection) => midi_connection,
        _ => exit_with_error(
            "Could not obtain connection to any midi device.",
            EXIT_CODE_NO_MIDI_DEVICE_FOUND,
            None,
        ),
    };

    print_debug_message("Midi connection set up.");
    print_debug_message(format!("Setting up socket listener on '{}'...", &socket_address).as_str());

    let socket_listener = match UnixListener::bind(&socket_address) {
        Ok(socket_listener) => socket_listener,
        Err(error) => exit_with_error(
            format!("Could not bind to address '{}'.", &socket_address).as_str(),
            EXIT_CODE_UNIX_SOCKET_ERROR,
            Some(Box::new(error)),
        ),
    };

    print_debug_message("Socket listener set up. Waiting for connections...");

    for socket_stream in socket_listener.incoming() {
        let socket_stream = match socket_stream {
            Ok(socket_stream) => socket_stream,
            Err(error) => exit_with_error(
                "An error occured while handling socket stream.",
                EXIT_CODE_UNIX_SOCKET_ERROR,
                Some(Box::new(error)),
            ),
        };

        match handle_incoming_socket_stream(socket_stream, &mut midi_connection) {
            Err(error @ TransmissionError::MidiStreamUnwritableError) => exit_with_error(
                "Could not write to midi device.",
                EXIT_CODE_MIDI_TRANSMISSION_ERROR,
                Some(Box::new(error)),
            ),
            Err(error @ TransmissionError::UnixStreamUnreadableError) => exit_with_error(
                "Could not read socket stream to buffer.",
                EXIT_CODE_UNIX_SOCKET_ERROR,
                Some(Box::new(error)),
            ),
            _ => (),
        };
    }

    midi_connection.close();
}

fn handle_incoming_socket_stream(
    mut stream: UnixStream,
    midi_connection: &mut MidiOutputConnection,
) -> Result<(), TransmissionError> {
    /* No locking necessary as this all is handled synchronously. This really makes sense in our use-case. Yay! */
    print_debug_message("Handling incoming connection...");

    let mut buffer: [u8; MAX_SYSEX_MESSAGE_LENGTH] = [0; MAX_SYSEX_MESSAGE_LENGTH];

    let buffer_length = match stream.read(&mut buffer) {
        Ok(buffer_length) => buffer_length,
        Err(_) => return Err(TransmissionError::UnixStreamUnreadableError),
    };

    print_debug_message(format!("Read {} bytes from stream.", buffer_length).as_str());
    print_debug_message("Sending data to midi device...");

    if midi_connection.send(&buffer[0..buffer_length]).is_err() {
        return Err(TransmissionError::MidiStreamUnwritableError);
    }

    print_debug_message("Sent data to midi device. Handled connection successfully.");
    Ok(())
}

fn get_socket_address() -> Option<String> {
    let command_args = env::args().collect::<Vec<String>>();
    match command_args.len() == 2 {
        true => Some(String::from(&command_args[1])),
        false => None,
    }
}

fn get_midi_output_connection() -> Option<MidiOutputConnection> {
    let midi_output = MidiOutput::new("midi_file_handle").ok()?;

    let midi_output_ports = midi_output.ports();

    let midi_output_port = midi_output_ports.last()?;

    midi_output
        .connect(midi_output_port, "midi_file_handle")
        .ok()
}

fn print_debug_message(message: &str) {
    if !cfg!(debug_assertions) {
        return;
    }

    println!("DBG: {}", message);
}

fn exit_with_error(message: &str, exit_code: i32, error: Option<Box<dyn Display>>) -> ! {
    if let Some(error) = error {
        if cfg!(debug_assertions) {
            eprintln!("ERR: {}", error);
        }
    };

    eprintln!("ERR: {}", message);

    exit(exit_code);
}
