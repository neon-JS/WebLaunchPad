mod errors;

use std::fs::File;
use gif::{DecodeOptions, ColorOutput, Frame, Encoder, Repeat};
use crate::errors::GifHelperError;
use crate::errors::GifHelperError::{FrameNotExisting, FrameNotReadable, InfoNotReadable, PaletteNotExisting};

/* First row of Launchpad should not be used. Therefore always add a black row for each frame. */
const HEIGHT_OFFSET: usize = 1;
const HEIGHT: usize = 8;
const WIDTH: usize = 8;

fn main() {
    let (input_file_path, output_file_path) = match get_file_paths() {
        None => {
            eprintln!("Missing paths.\nUsage: ./gif_helper <input file> <output file>");
            return;
        }
        Some(paths) => paths
    };

    let input_file = match File::open(input_file_path) {
        Ok(f) => f,
        Err(e) => {
            eprintln!("Could not open input file.\nError: {}", e);
            return;
        }
    };

    let mut output_file = match File::create(output_file_path) {
        Ok(f) => f,
        Err(e) => {
            eprintln!("Could not open output file.\nError: {}", e);
            return;
        }
    };

    let (gif, palette) = match get_gif_frame_and_palette(&input_file) {
        Ok(f) => f,
        Err(e) => {
            eprintln!("Could not get GIF content.\nError: {}", e);
            return;
        }
    };

    let mut encoder = match Encoder::new(
        &mut output_file,
        WIDTH as u16,
        HEIGHT as u16,
        &palette,
    ) {
        Ok(e) => e,
        Err(e) => {
            eprintln!("Could not create GIF encoder.\nError: {}", e);
            return;
        }
    };

    match encoder.set_repeat(Repeat::Infinite) {
        Ok(_) => (),
        Err(e) => {
            eprintln!("Could not write GIF repeat.\nError: {}", e);
            return;
        }
    }

    let offset_upper_bound = (gif.width as usize) - (WIDTH - 1);
    for offset in 0..offset_upper_bound {
        let frame = calculate_frame_by_offset(offset, &gif);

        match encoder.write_frame(&frame) {
            Ok(_) => (),
            Err(e) => {
                eprintln!("Could not write GIF frame to file.\nError: {}", e);
                return;
            }
        }
    }
}

fn calculate_frame_by_offset<'a>(offset: usize, gif: &'a Frame<'a>) -> Frame<'a> {
    let mut frame_buffer: Vec<u8> = Vec::new();

    for _ in 0..HEIGHT_OFFSET {
        for _ in 0..WIDTH {
            for _ in 0..3 {
                frame_buffer.push(0);
            }
            frame_buffer.push(255);
        }
    }

    for y in 0..HEIGHT {
        for x in 0..WIDTH {
            let pixel_index = x + offset + (y * (gif.width as usize));
            for byte in 0..4 {
                let buffer_index = pixel_index * 4 + byte;
                frame_buffer.push(gif.buffer[buffer_index]);
            }
        }
    }

    let mut frame = Frame::from_rgba(
        WIDTH as u16,
        (HEIGHT + HEIGHT_OFFSET) as u16,
        frame_buffer.as_mut_slice(),
    );
    frame.delay = 100;

    frame
}

fn get_file_paths() -> Option<(String, String)> {
    let args = std::env::args()
        .skip(1)
        .collect::<Vec<String>>();

    if args.len() < 2 {
        None
    } else {
        Some((
            args[0].clone(),
            args[1].clone()
        ))
    }
}

fn get_gif_frame_and_palette(file: &File) -> Result<(Frame, Vec<u8>), GifHelperError> {
    let mut options = DecodeOptions::new();
    options.set_color_output(ColorOutput::RGBA);

    let mut decoder = match options.read_info(file) {
        Ok(d) => d,
        Err(_) => {
            return Err(InfoNotReadable);
        }
    };

    let palette = match decoder.palette() {
        Ok(p) => p.to_vec(),
        Err(_) => {
            return Err(PaletteNotExisting);
        }
    };

    let frame = match decoder.read_next_frame() {
        Ok(f) => f,
        Err(_) => {
            return Err(FrameNotReadable);
        }
    };

    match frame {
        None => Err(FrameNotExisting),
        Some(f) => Ok((f.clone(), palette))
    }
}
