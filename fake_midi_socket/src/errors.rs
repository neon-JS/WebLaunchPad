use std::fmt;

#[derive(Debug, Clone)]
pub enum TransmissionError {
    UnixStreamUnreadableError,
    MidiStreamUnwritableError,
}

impl fmt::Display for TransmissionError {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            TransmissionError::UnixStreamUnreadableError => {
                write!(f, "Could not read from unix socket stream")
            }
            TransmissionError::MidiStreamUnwritableError => {
                write!(f, "Could not write to midi stream")
            }
        }
    }
}
