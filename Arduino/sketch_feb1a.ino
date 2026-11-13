#include <Wire.h>
#include <MadgwickAHRS.h>

// ===== MPU9250 =====
#define MPU_ADDR 0x68
#define SDA_PIN 21
#define SCL_PIN 22

Madgwick filter;

// ===== FLEX SENSOR PINS =====
const int thumbMCP  = 32;
const int thumbPIP  = 33;
const int indexMCP  = 34;
const int indexPIP  = 25;
const int middleMCP = 26;
const int middlePIP = 27;
const int ringMCP   = 14;
const int ringPIP   = 12;
const int pinkyMCP  = 13;
const int pinkyPIP  = 4;

// ===== Fixed resistors =====
const float R_10K = 10000.0;
const float R_22K = 22000.0;

// ===== ADC constants =====
const float VCC = 3.3;
const float ADC_MAX = 4095.0;

// ----- Flex sensor read helper -----
float readFlexResistance(int pin, float R_FIXED) {
  int adc = analogRead(pin);
  float voltage = adc * (VCC / ADC_MAX);
  if (voltage < 0.001) return 0; // prevent divide-by-zero
  return R_FIXED * (VCC / voltage - 1.0);
}

void setup() {
  // Serial for Unity
  Serial.begin(115200);

  // ===== ESP32 ADC setup =====
  analogReadResolution(12);
  analogSetAttenuation(ADC_11db); // 0–3.3V range

  // ===== I2C setup for MPU =====
  Wire.begin(SDA_PIN, SCL_PIN);

  // Wake up MPU9250
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x6B);
  Wire.write(0);
  Wire.endTransmission(true);

  // Start Madgwick
  filter.begin(50); // 50 Hz
}

void loop() {
  // --------- Read MPU9250 ---------
  int16_t ax, ay, az, gx, gy, gz;

  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x3B); // ACCEL_XOUT_H
  Wire.endTransmission(false);
  Wire.requestFrom(MPU_ADDR, 14, true);

  ax = Wire.read() << 8 | Wire.read();
  ay = Wire.read() << 8 | Wire.read();
  az = Wire.read() << 8 | Wire.read();

  int16_t temp = Wire.read() << 8 | Wire.read(); // temperature
  gx = Wire.read() << 8 | Wire.read();
  gy = Wire.read() << 8 | Wire.read();
  gz = Wire.read() << 8 | Wire.read();

  // Convert to g / rad/s
  float aX = ax / 16384.0;
  float aY = ay / 16384.0;
  float aZ = az / 16384.0;

  float gX = gx * (3.14159 / 180.0) / 131.0;
  float gY = gy * (3.14159 / 180.0) / 131.0;
  float gZ = gz * (3.14159 / 180.0) / 131.0;

  filter.updateIMU(gX, gY, gZ, aX, aY, aZ);

  // --------- Read all flex sensors ---------
  float thumbMCP_R  = readFlexResistance(thumbMCP,  R_10K);
  float thumbPIP_R  = readFlexResistance(thumbPIP,  R_10K);
  float indexMCP_R  = readFlexResistance(indexMCP,  R_22K);
  float indexPIP_R  = readFlexResistance(indexPIP,  R_10K);
  float middleMCP_R = readFlexResistance(middleMCP, R_22K);
  float middlePIP_R = readFlexResistance(middlePIP, R_10K);
  float ringMCP_R   = readFlexResistance(ringMCP,   R_10K);
  float ringPIP_R   = readFlexResistance(ringPIP,   R_10K);
  float pinkyMCP_R  = readFlexResistance(pinkyMCP,  R_10K);
  float pinkyPIP_R  = readFlexResistance(pinkyPIP,  R_10K);

  // --------- Serial output ---------
  // Format: q0,q1,q2,q3,thumbMCP,thumbPIP,indexMCP,indexPIP,...,pinkyPIP
  Serial.print(filter.q0, 6); Serial.print(",");
  Serial.print(filter.q1, 6); Serial.print(",");
  Serial.print(filter.q2, 6); Serial.print(",");
  Serial.print(filter.q3, 6); Serial.print(",");

  Serial.print(thumbMCP_R);  Serial.print(",");
  Serial.print(thumbPIP_R);  Serial.print(",");
  Serial.print(indexMCP_R);  Serial.print(",");
  Serial.print(indexPIP_R);  Serial.print(",");
  Serial.print(middleMCP_R); Serial.print(",");
  Serial.print(middlePIP_R); Serial.print(",");
  Serial.print(ringMCP_R);   Serial.print(",");
  Serial.print(ringPIP_R);   Serial.print(",");
  Serial.print(pinkyMCP_R);  Serial.print(",");
  Serial.println(pinkyPIP_R);

  delay(10); // ~100 Hz
}
