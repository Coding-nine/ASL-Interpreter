import serial
import socket

# ===== SERIAL =====
SERIAL_PORT = "COM5"
BAUD_RATE = 115200          # must match Arduino
ser = serial.Serial(
    port=SERIAL_PORT,
    baudrate=BAUD_RATE,
    timeout=1
)

# ===== UDP =====
UDP_IP = "127.0.0.1"        # Unity localhost
UDP_PORT = 5005
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

print("Python serial → UDP bridge running")

while True:
    try:
        raw = ser.readline()

        if not raw:
            continue

        # Arduino must send ASCII only (numbers, commas, newline)
        line = raw.decode("ascii", errors="ignore").strip()

        if line:
            sock.sendto(line.encode("ascii"), (UDP_IP, UDP_PORT))

    except Exception as e:
        print("Bridge error:", e)
