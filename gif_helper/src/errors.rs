use std::fmt;
use std::fmt::Formatter;

pub enum GifHelperError {
    InfoNotReadable,
    PaletteNotExisting,
    FrameNotExisting,
    FrameNotReadable,
}

impl fmt::Display for GifHelperError {
    fn fmt(&self, f: &mut Formatter<'_>) -> fmt::Result {
        match self {
            GifHelperError::InfoNotReadable => write!(f, "Could not read GIF information"),
            GifHelperError::PaletteNotExisting => write!(f, "Color palette not existing"),
            GifHelperError::FrameNotReadable => write!(f, "Frame not readable"),
            GifHelperError::FrameNotExisting => write!(f, "No frame existing"),
        }
    }
}