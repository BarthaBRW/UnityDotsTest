﻿using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct NoiseJob : IJobParallelFor {
   [WriteOnly] public NativeArray<float> levels;
   [ReadOnly] public int size;
   [ReadOnly] public float noiseScale;
   [ReadOnly] public float noiseOffset;
   [ReadOnly] public Vector3 offset;

   public void Execute(int index) {
      int x = index % size;
      int y = index / size % size;
      int z = index / (size * size);
      float fx = (x + offset.x + noiseOffset) / noiseScale;
      float fy = (y + offset.y) / noiseScale;
      float fz = (z + offset.z) / noiseScale;
      levels[index] = PerlinNoise3D(fx, fy, fz);
   }

   private static float PerlinNoise3D(float x, float y, float z) {
      float xy = _perlin3DFixed(x, y);
      float xz = _perlin3DFixed(x, z);
      float yz = _perlin3DFixed(y, z);
      float yx = _perlin3DFixed(y, x);
      float zx = _perlin3DFixed(z, x);
      float zy = _perlin3DFixed(z, y);
      return xy * xz * yz * yx * zx * zy;
   }

   private static float _perlin3DFixed(float a, float b) {
      return Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
   }


   // Low precision Sin approximation
   // http://www.mclimatiano.com/faster-sine-approximation-using-quadratic-curve/
   private static float Sin(float x) {
      if (x < -3.14159265f)
         x += 6.28318531f;
      else if (x > 3.14159265f)
         x -= 6.28318531f;

      if (x < 0)
         return x * (1.27323954f + 0.405284735f * x);
      return x * (1.27323954f - 0.405284735f * x);
   }
}
