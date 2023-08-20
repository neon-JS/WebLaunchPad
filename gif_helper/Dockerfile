FROM rust:1.71

WORKDIR /app

COPY src src
COPY Cargo.lock .
COPY Cargo.toml .

RUN cargo install --path .

CMD ["gif_helper"]